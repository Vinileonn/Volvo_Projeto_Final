using System.ComponentModel.DataAnnotations;

namespace cinecore.DTOs.Usuario
{
    /// <summary>
    /// DTO para atualização de um usuário existente
    /// </summary>
    public class AtualizarUsuarioDto
    {
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public string? Nome { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(200, ErrorMessage = "O email deve ter no máximo 200 caracteres")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }

        [StringLength(500, ErrorMessage = "O endereço deve ter no máximo 500 caracteres")]
        public string? Endereco { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }
    }
}
