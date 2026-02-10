using System.ComponentModel.DataAnnotations;

namespace cinecore.Models
{
    /// <summary>
    /// Modelo de dados para representar um cinema na WebAPI
    /// </summary>
    public class Cinema
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 255 caracteres")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O endereço deve ter entre 1 e 500 caracteres")]
        public required string Endereco { get; set; }

        public List<Sala> Salas { get; set; } = new List<Sala>();

        public List<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public Cinema() { }

        public Cinema(int id, string nome, string endereco)
        {
            Id = id;
            Nome = nome;
            Endereco = endereco;
            Salas = new List<Sala>();
            Funcionarios = new List<Funcionario>();
            DataCriacao = DateTime.Now;
        }
    }
}
