using System.ComponentModel.DataAnnotations;

namespace cinecore.DTOs.Usuario
{
    /// <summary>
    /// DTO para criar um novo usuário (sem ID)
    /// </summary>
    public class CriarUsuarioDto
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

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter entre 11 e 14 caracteres")]
        public required string CPF { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public required string Telefone { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(500, ErrorMessage = "O endereço deve ter no máximo 500 caracteres")]
        public required string Endereco { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        public required DateTime DataNascimento { get; set; }
    }
}
