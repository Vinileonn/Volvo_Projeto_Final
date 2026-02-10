using System.ComponentModel.DataAnnotations;
using cinecore.Enums;

namespace cinecore.Models
{
    /// <summary>
    /// Modelo de dados para representar um produto alimentício na WebAPI
    /// </summary>
    public class ProdutoAlimento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public required string Nome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        public CategoriaProduto? Categoria { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço não pode ser negativo")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O estoque atual é obrigatório")]
        [Range(0, int.MaxValue, ErrorMessage = "O estoque atual não pode ser negativo")]
        public int EstoqueAtual { get; set; }

        [Required(ErrorMessage = "O estoque mínimo é obrigatório")]
        [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo não pode ser negativo")]
        public int EstoqueMinimo { get; set; }

        public bool EhTematico { get; set; } = false;

        [StringLength(200, ErrorMessage = "O tema do filme deve ter no máximo 200 caracteres")]
        public string? TemaFilme { get; set; }

        public bool EhCortesia { get; set; } = false;

        public bool ExclusivoPreEstreia { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public ProdutoAlimento() { }

        public ProdutoAlimento(int id, string nome, string? descricao, CategoriaProduto? categoria,
            decimal preco, int estoqueAtual, int estoqueMinimo, bool ehTematico = false,
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
            DataCriacao = DateTime.Now;
        }
    }
}
