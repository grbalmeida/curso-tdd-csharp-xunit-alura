using System;
using System.Collections.Generic;
using System.Linq;

namespace Alura.LeilaoOnline.Core
{
    public enum EstadoLeilao
    {
        LeilaoAntesDoPregao,
        LeilaoEmAndamento,
        LeilaoFinalizado
    }

    public class Leilao
    {
        private Interessada _ultimoCliente;
        private IList<Lance> _lances;
        public IEnumerable<Lance> Lances => _lances;
        public string Peca { get; }
        public Lance Ganhador { get; private set; }
        public EstadoLeilao Estado { get; private set; }
        public double ValorDestino { get; }

        public Leilao(string peca, double valorDestino = 0)
        {
            Peca = peca;
            _lances = new List<Lance>();
            Estado = EstadoLeilao.LeilaoAntesDoPregao;
            ValorDestino = valorDestino;
        }

        private bool NovoLanceAceito(Interessada cliente)
        {
            return Estado == EstadoLeilao.LeilaoEmAndamento && cliente != _ultimoCliente;
        }

        public void RecebeLance(Interessada cliente, double valor)
        {
            if (NovoLanceAceito(cliente))
            {
                _lances.Add(new Lance(cliente, valor));
                _ultimoCliente = cliente;
            }
        }

        public void IniciaPregao()
        {
            Estado = EstadoLeilao.LeilaoEmAndamento;
        }

        public void TerminaPregao()
        {
            if (Estado != EstadoLeilao.LeilaoEmAndamento)
            {
                throw new InvalidOperationException(
                    "Não é possível terminar o pregão sem que ele tenha começado. Para isso, utilize o método IniciaPregao()."
                );
            }

            if (ValorDestino > 0)
            {
                // Modalidade oferta superior mais próxima
                Ganhador = Lances
                    .DefaultIfEmpty(new Lance(null, 0))
                    .Where(l => l.Valor > ValorDestino)
                    .OrderBy(l => l.Valor)
                    .FirstOrDefault();
            }
            else
            {
                // Modalidade maior valor
                Ganhador = _lances
                    .DefaultIfEmpty(new Lance(null, 0))
                    .OrderBy(l => l.Valor)
                    .LastOrDefault();
            }

            Estado = EstadoLeilao.LeilaoFinalizado;
        }
    }
}
