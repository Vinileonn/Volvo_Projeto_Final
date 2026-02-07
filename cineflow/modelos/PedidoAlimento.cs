using System.Text;
using cineflow.enumeracoes;
using cineflow.utilitarios;

namespace cineflow.modelos
{
    public class PedidoAlimento
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public float ValorTotal { get; set; }
        public List<ItemPedidoAlimento> Itens { get; set; }

        public FormaPagamento? FormaPagamento { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorTroco { get; set; }
        public Dictionary<decimal, int> TrocoDetalhado { get; set; }

        // ANTIGO:
        // public PedidoAlimento(int id, float valorTotal)
        // {
        //     Id = id;
        //     DataPedido = DateTime.Now;
        //     ValorTotal = valorTotal;
        //     Itens = new List<ItemPedidoAlimento>();
        // }

        // Guarda pagamento e troco no proprio pedido para consulta rapida.
        public PedidoAlimento(int id, float valorTotal)
        {
            Id = id;
            DataPedido = DateTime.Now;
            ValorTotal = valorTotal;
            Itens = new List<ItemPedidoAlimento>();
            TrocoDetalhado = new Dictionary<decimal, int>();
        }

        public void AdicionarItem(ItemPedidoAlimento item)
        {
            Itens.Add(item);
            ValorTotal += item.Preco * item.Quantidade;
        }

        // ANTIGO:
        // public override string ToString()
        // {
        //     StringBuilder sb = new StringBuilder();
        //     sb.AppendLine($"Pedido ID: {Id}");
        //     sb.AppendLine($"Data do Pedido: {FormatadorData.FormatarDataComHora(DataPedido)}");
        //     sb.AppendLine($"Valor Total: {FormatadorMoeda.Formatar(ValorTotal)}");
        //     sb.AppendLine("Itens do Pedido:");
        //     foreach (var item in Itens)
        //     {
        //         sb.AppendLine(item.ToString());
        //     }
        //     return sb.ToString();
        // }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Pedido ID: {Id}");
            sb.AppendLine($"Data do Pedido: {FormatadorData.FormatarDataComHora(DataPedido)}");
            sb.AppendLine($"Valor Total: {FormatadorMoeda.Formatar(ValorTotal)}");

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

            sb.AppendLine("Itens do Pedido:");
            foreach (var item in Itens)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }
    }
}




