using System.Text;
using cinema.enumeracoes;
using cinema.utilitarios;
namespace cinema.modelos
{
    public class ProdutoAlimento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public CategoriaProduto? Categoria { get; set; }

        public float Preco { get; set; }

        public int EstoqueAtual { get; set; }

        public int EstoqueMinimo { get; set; }

        public ProdutoAlimento(int id, string nome, string? descricao, CategoriaProduto? categoria,
                              float preco, int estoqueAtual, int estoqueMinimo)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            Categoria = categoria;
            Preco = preco;
            EstoqueAtual = estoqueAtual;
            EstoqueMinimo = estoqueMinimo;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Produto ID: {Id}");
            sb.AppendLine($"Nome: {Nome}");
            sb.AppendLine($"Descrição: {Descricao}");
            sb.AppendLine($"Categoria: {Categoria}");
            sb.AppendLine($"Preço: {FormatadorMoeda.Formatar(Preco)}");
            sb.AppendLine($"Estoque Atual: {EstoqueAtual}");
            sb.AppendLine($"Estoque Mínimo: {EstoqueMinimo}");
            return sb.ToString();
        }
    }
}




