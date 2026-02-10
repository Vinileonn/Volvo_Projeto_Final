using System.ComponentModel.DataAnnotations;
using cinecore.Enums;

namespace cinecore.DTOs.ProdutoAlimento
{
    /// <summary>
    /// DTO para atualização de um ProdutoAlimento existente
    /// </summary>
    public class AtualizarProdutoAlimentoDto
    {
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public string? Nome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        public CategoriaProduto? Categoria { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O preço não pode ser negativo")]
        public decimal? Preco { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo não pode ser negativo")]
        public int? EstoqueMinimo { get; set; }

        public bool? EhTematico { get; set; }

        [StringLength(200, ErrorMessage = "O tema do filme deve ter no máximo 200 caracteres")]
        public string? TemaFilme { get; set; }

        public bool? EhCortesia { get; set; }

        public bool? ExclusivoPreEstreia { get; set; }
    }
}
