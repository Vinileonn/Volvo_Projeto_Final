using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.DTOs.AluguelSala
{
    /// <summary>
    /// DTO para atualização de um Aluguel de Sala existente
    /// </summary>
    public class AtualizarAluguelSalaDto
    {
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public string? NomeCliente { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "O contato deve ter entre 1 e 100 caracteres")]
        public string? Contato { get; set; }

        public DateTime? Inicio { get; set; }

        public DateTime? Fim { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "O motivo deve ter entre 1 e 500 caracteres")]
        public string? Motivo { get; set; }

        public bool? PacoteAniversario { get; set; }
    }
}
