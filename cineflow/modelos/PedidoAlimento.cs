using System.Text;
using cineflow.enumeracoes;
using cineflow.modelos.UsuarioModelo;
using cineflow.utilitarios;

namespace cineflow.modelos
{
    public class PedidoAlimento
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public float ValorTotal { get; set; }
        public List<ItemPedidoAlimento> Itens { get; set; }

        public Cliente? Cliente { get; set; }
        public float ValorDesconto { get; set; }
        public string? MotivoDesconto { get; set; }

        public FormaPagamento? FormaPagamento { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorTroco { get; set; }
        public Dictionary<decimal, int> TrocoDetalhado { get; set; }
        public int PontosUsados { get; set; }
        public int PontosGerados { get; set; }
        public float TaxaCancelamento { get; set; }

        // Guarda pagamento e troco no proprio pedido para consulta rapida.
        public PedidoAlimento(int id, float valorTotal)
        {
            Id = id;
            DataPedido = DateTime.Now;
            ValorTotal = valorTotal;
            Itens = new List<ItemPedidoAlimento>();
            TrocoDetalhado = new Dictionary<decimal, int>();
            PontosUsados = 0;
            PontosGerados = 0;
            TaxaCancelamento = 0f;
        }

        public void AdicionarItem(ItemPedidoAlimento item)
        {
            Itens.Add(item);
            ValorTotal += item.Preco * item.Quantidade;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Pedido ID: {Id}");
            sb.AppendLine($"Data do Pedido: {FormatadorData.FormatarDataComHora(DataPedido)}");
            sb.AppendLine($"Valor Total: {FormatadorMoeda.Formatar(ValorTotal)}");
            if (ValorDesconto > 0)
            {
                sb.AppendLine($"Desconto: {FormatadorMoeda.Formatar(ValorDesconto)}");
                if (!string.IsNullOrWhiteSpace(MotivoDesconto))
                {
                    sb.AppendLine($"Motivo: {MotivoDesconto}");
                }
            }

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

            if (PontosUsados > 0)
            {
                sb.AppendLine($"Pontos usados: {PontosUsados}");
            }

            if (PontosGerados > 0)
            {
                sb.AppendLine($"Pontos gerados: {PontosGerados}");
            }

            if (TaxaCancelamento > 0)
            {
                sb.AppendLine($"Taxa de cancelamento: {FormatadorMoeda.Formatar(TaxaCancelamento)}");
            }

            sb.AppendLine("Itens do Pedido:");
            foreach (var item in Itens)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }
    }
}




