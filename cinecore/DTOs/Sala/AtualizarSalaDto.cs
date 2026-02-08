using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.DTOs.Sala
{
    /// <summary>
    /// DTO para atualização de uma Sala existente
    /// </summary>
    public class AtualizarSalaDto
    {
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 100 caracteres")]
        public string? Nome { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser maior que zero")]
        public int? Capacidade { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de assentos para casal não pode ser negativa")]
        public int? QuantidadeAssentosCasal { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de assentos PCD não pode ser negativa")]
        public int? QuantidadeAssentosPCD { get; set; }
    }
}
