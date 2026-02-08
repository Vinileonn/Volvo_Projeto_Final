using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados para representar uma sala de cinema na WebAPI
    /// </summary>
    public class Sala
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 100 caracteres")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "A capacidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser maior que zero")]
        public int Capacidade { get; set; }

        [Required(ErrorMessage = "O tipo de sala é obrigatório")]
        public TipoSala Tipo { get; set; } = TipoSala.Normal;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de assentos para casal não pode ser negativa")]
        public int QuantidadeAssentosCasal { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de assentos PCD não pode ser negativa")]
        public int QuantidadeAssentosPCD { get; set; } = 0;

        public Cinema? Cinema { get; set; }

        public List<Assento> Assentos { get; set; } = new List<Assento>();

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public Sala() { }

        public Sala(int id, string nome, int capacidade, Cinema? cinema = null, TipoSala tipo = TipoSala.Normal,
            int quantidadeAssentosCasal = 0, int quantidadeAssentosPCD = 0)
        {
            Id = id;
            Nome = nome;
            Capacidade = capacidade;
            Cinema = cinema;
            Tipo = tipo;
            QuantidadeAssentosCasal = quantidadeAssentosCasal;
            QuantidadeAssentosPCD = quantidadeAssentosPCD;
            Assentos = new List<Assento>();
            DataCriacao = DateTime.Now;
        }

        public Assento? ConsultarAssento(char fila, int numero)
        {
            foreach (var assento in Assentos)
            {
                if (assento.Fila == fila && assento.Numero == numero)
                {
                    return assento;
                }
            }
            return null;
        }
    }
}
