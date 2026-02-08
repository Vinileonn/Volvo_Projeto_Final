using System.Reflection.Metadata;
using System.Text;
using cineflow.enumeracoes;
using cineflow.modelos;
using cineflow.modelos.UsuarioModelo;
using cineflow.utilitarios;
namespace cineflow.modelos.IngressoModelo
{
    public abstract class Ingresso
    {
        public int Id { get; set; }
        
        public char Fila { get; set; } // Exemplo: 'A', 'B', etc.
        public int Numero { get; set; } // Exemplo: 10, 5, etc.

        public Sessao Sessao { get; set; }
        public Cliente Cliente { get; set; }
        public Assento Assento { get; set; }

        public DateTime DataCompra { get; set; }

        public FormaPagamento? FormaPagamento { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorTroco { get; set; }
        public Dictionary<decimal, int> TrocoDetalhado { get; set; }

        public Ingresso(int id, char fila, int numero, Sessao sessao, Cliente cliente, Assento assento, DateTime dataCompra)
        {
            Id = id;
            Fila = fila;
            Numero = numero;
            Sessao = sessao;
            Cliente = cliente;
            Assento = assento;
            DataCompra = dataCompra;
            TrocoDetalhado = new Dictionary<decimal, int>();
        }

        //métodos
        public abstract float CalcularPreco(float precoBase);
        public abstract string ObterTipo();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes do Ingresso:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Sessao ID: {Sessao.Id}");
            sb.AppendLine($"Data da Compra: {FormatadorData.FormatarDataComHora(DataCompra)}");
            sb.AppendLine($"Numero do Lugar: {Fila}{Numero}");

            if (FormaPagamento.HasValue)
            {
                sb.AppendLine($"Pagamento: {FormaPagamento}");
                sb.AppendLine($"Valor Pago: {FormatadorMoeda.Formatar(ValorPago)}");
                if (ValorTroco > 0)
                {
                    sb.AppendLine($"Troco: {FormatadorMoeda.Formatar(ValorTroco)}");
                    sb.AppendLine("Troco Detalhado:");
                    foreach (var kvp in TrocoDetalhado)
                    {
                        sb.AppendLine($"{FormatadorMoeda.Formatar(kvp.Key)} x {kvp.Value}");
                    }
                }
            }

            return sb.ToString();
        }
    }
}




