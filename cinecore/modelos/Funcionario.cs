using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados para representar um funcionário na WebAPI
    /// </summary>
    public class Funcionario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O cargo é obrigatório")]
        public CargoFuncionario Cargo { get; set; }

        public Cinema? Cinema { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public Funcionario() { }

        public Funcionario(int id, string nome, CargoFuncionario cargo, Cinema? cinema = null)
        {
            Id = id;
            Nome = nome;
            Cargo = cargo;
            Cinema = cinema;
            DataCriacao = DateTime.Now;
        }
    }
}
