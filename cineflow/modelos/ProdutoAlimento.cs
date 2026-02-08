using System.Text;
using cineflow.enumeracoes;
using cineflow.utilitarios;
namespace cineflow.modelos
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

        public bool EhTematico { get; set; }
        public string? TemaFilme { get; set; }
        public bool EhCortesia { get; set; }
        public bool ExclusivoPreEstreia { get; set; }

        public ProdutoAlimento(int id, string nome, string? descricao, CategoriaProduto? categoria,
                              float preco, int estoqueAtual, int estoqueMinimo, bool ehTematico = false,
                              string? temaFilme = null, bool ehCortesia = false, bool exclusivoPreEstreia = false)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            Categoria = categoria;
            Preco = preco;
            EstoqueAtual = estoqueAtual;
            EstoqueMinimo = estoqueMinimo;
            EhTematico = ehTematico;
            TemaFilme = temaFilme;
            EhCortesia = ehCortesia;
            ExclusivoPreEstreia = exclusivoPreEstreia;
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
            if (EhTematico)
            {
                sb.AppendLine($"Temático: Sim ({TemaFilme ?? "Sem tema"})");
            }
            if (EhCortesia)
            {
                sb.AppendLine("Cortesia: Sim");
            }
            if (ExclusivoPreEstreia)
            {
                sb.AppendLine("Exclusivo Pré-Estreia: Sim");
            }
            return sb.ToString();
        }
    }
}




