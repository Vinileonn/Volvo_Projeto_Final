using System.ComponentModel.DataAnnotations;

namespace cinecore.DTOs.Usuario
{
    /// <summary>
    /// DTO para criar um novo administrador
    /// </summary>
    public class CriarAdministradorDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(200, ErrorMessage = "O email deve ter no máximo 200 caracteres")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 200 caracteres")]
        public required string Senha { get; set; }
    }
}
