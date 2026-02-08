using System.ComponentModel.DataAnnotations;

namespace cinecore.DTOs.ProdutoAlimento
{
    /// <summary>
    /// DTO para atualização de estoque de um ProdutoAlimento
    /// </summary>
    public class AtualizarEstoqueDto
    {
        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
    }
}
