using System.ComponentModel.DataAnnotations;

namespace cinecore.DTOs.Cinema
{
    /// <summary>
    /// DTO para criação de um novo Cinema
    /// </summary>
    public class CriarCinemaDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 255 caracteres")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O endereço deve ter entre 1 e 500 caracteres")]
        public required string Endereco { get; set; }
    }
}
