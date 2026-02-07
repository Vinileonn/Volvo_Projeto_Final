using System.Text;
using cinema.utilitarios;

namespace cinema.modelos
{
    public class ItemPedidoAlimento
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public float Preco { get; set; }

        public ProdutoAlimento? Produto { get; set; }

        public ItemPedidoAlimento(int id, ProdutoAlimento produto, int quantidade, float preco)
        {
            Id = id;
            Produto = produto;
            Quantidade = quantidade;
            Preco = preco;
        }

        private float CalcularSubtotal()
        {
            return Preco * Quantidade;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Item ID: {Id}");
            sb.AppendLine($"Produto: {Produto?.Nome}");
            sb.AppendLine($"Quantidade: {Quantidade}");
            sb.AppendLine($"Preço Unitário: {FormatadorMoeda.Formatar(Preco)}");
            sb.AppendLine($"Subtotal: {FormatadorMoeda.Formatar(CalcularSubtotal())}");
            return sb.ToString();
        }
    }
}




