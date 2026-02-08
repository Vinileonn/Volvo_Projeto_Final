using System.ComponentModel.DataAnnotations;

namespace cinecore.DTOs.Cinema
{
    /// <summary>
    /// DTO para atualização de um Cinema existente
    /// </summary>
    public class AtualizarCinemaDto
    {
        [StringLength(255, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 255 caracteres")]
        public string? Nome { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "O endereço deve ter entre 1 e 500 caracteres")]
        public string? Endereco { get; set; }
    }
}
