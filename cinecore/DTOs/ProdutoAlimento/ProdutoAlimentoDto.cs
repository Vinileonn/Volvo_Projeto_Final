using cinecore.Enums;

namespace cinecore.DTOs.ProdutoAlimento
{
    /// <summary>
    /// DTO para retorno de dados de ProdutoAlimento
    /// </summary>
    public class ProdutoAlimentoDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public CategoriaProduto? Categoria { get; set; }
        public decimal Preco { get; set; }
        public int EstoqueAtual { get; set; }
        public int EstoqueMinimo { get; set; }
        public bool EhTematico { get; set; }
        public string? TemaFilme { get; set; }
        public bool EhCortesia { get; set; }
        public bool ExclusivoPreEstreia { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
