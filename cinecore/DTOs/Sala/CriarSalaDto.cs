using System.ComponentModel.DataAnnotations;
using cinecore.Enums;

namespace cinecore.DTOs.Sala
{
    /// <summary>
    /// DTO para criação de uma nova Sala
    /// </summary>
    public class CriarSalaDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 100 caracteres")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "A capacidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser maior que zero")]
        public int Capacidade { get; set; }

        [Required(ErrorMessage = "O tipo de sala é obrigatório")]
        public TipoSala Tipo { get; set; } = TipoSala.Normal;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de assentos para casal não pode ser negativa")]
        public int QuantidadeAssentosCasal { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de assentos PCD não pode ser negativa")]
        public int QuantidadeAssentosPCD { get; set; } = 0;

        [Required(ErrorMessage = "O cinema é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do cinema deve ser maior que zero")]
        public int CinemaId { get; set; }
    }
}
