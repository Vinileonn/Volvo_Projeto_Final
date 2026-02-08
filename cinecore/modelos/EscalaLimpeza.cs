using System.ComponentModel.DataAnnotations;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados para representar uma escala de limpeza na WebAPI
    /// </summary>
    public class EscalaLimpeza
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória")]
        public DateTime Inicio { get; set; }

        [Required(ErrorMessage = "A data de fim é obrigatória")]
        public DateTime Fim { get; set; }

        public Sala? Sala { get; set; }

        public Funcionario? Funcionario { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public EscalaLimpeza() { }

        public EscalaLimpeza(int id, DateTime inicio, DateTime fim, Sala? sala = null,
            Funcionario? funcionario = null)
        {
            Id = id;
            Inicio = inicio;
            Fim = fim;
            Sala = sala;
            Funcionario = funcionario;
            DataCriacao = DateTime.Now;
        }
    }
}
