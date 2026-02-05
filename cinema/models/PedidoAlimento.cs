using System.Text;
using cinema.utils;

namespace cinema.models
{
    public class PedidoAlimento
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public float ValorTotal { get; set; }
        public List<ItemPedidoAlimento> Itens { get; set; }

        public PedidoAlimento(int id, float valorTotal)
        {
            Id = id;
            DataPedido = DateTime.Now;
            ValorTotal = valorTotal;
            Itens = new List<ItemPedidoAlimento>();
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
            sb.AppendLine("Itens do Pedido:");
            foreach (var item in Itens)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }
    }
}