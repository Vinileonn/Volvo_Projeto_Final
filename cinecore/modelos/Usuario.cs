using System.ComponentModel.DataAnnotations;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados abstrato para representar um usuário na WebAPI
    /// </summary>
    public abstract class Usuario
    {
        [Key]
        public int Id { get; set; }

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

        [Required(ErrorMessage = "A data de cadastro é obrigatória")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public Usuario() { }

        public Usuario(int id, string nome, string email, string senha, DateTime dataCadastro)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Senha = senha;
            DataCadastro = dataCadastro;
            DataCriacao = DateTime.Now;
        }
    }
}
