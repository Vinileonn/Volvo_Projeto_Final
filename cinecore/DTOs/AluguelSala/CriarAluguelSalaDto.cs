using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.DTOs.AluguelSala
{
    /// <summary>
    /// DTO para criação de um novo Aluguel de Sala
    /// </summary>
    public class CriarAluguelSalaDto
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public required string NomeCliente { get; set; }

        [Required(ErrorMessage = "O contato é obrigatório")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O contato deve ter entre 1 e 100 caracteres")]
        public required string Contato { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória")]
        public DateTime Inicio { get; set; }

        [Required(ErrorMessage = "A data de fim é obrigatória")]
        public DateTime Fim { get; set; }

        [Required(ErrorMessage = "O motivo é obrigatório")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O motivo deve ter entre 1 e 500 caracteres")]
        public required string Motivo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O valor não pode ser negativo")]
        public decimal Valor { get; set; } = 0m;

        public bool PacoteAniversario { get; set; } = false;

        [Required(ErrorMessage = "O ID da sala é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID da sala deve ser maior que zero")]
        public int SalaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do cliente deve ser maior que zero")]
        public int? ClienteId { get; set; }
    }
}
