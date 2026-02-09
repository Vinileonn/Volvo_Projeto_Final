using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados para representar um assento na WebAPI
    /// </summary>
    public class Assento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A fila é obrigatória")]
        public char Fila { get; set; }

        [Required(ErrorMessage = "O número é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O número deve ser maior que zero")]
        public int Numero { get; set; }

        public bool Disponivel { get; set; } = true;

        [Required(ErrorMessage = "O tipo de assento é obrigatório")]
        public TipoAssento Tipo { get; set; } = TipoAssento.Normal;

        [Required(ErrorMessage = "A quantidade de lugares é obrigatória")]
        [Range(1, 2, ErrorMessage = "A quantidade de lugares deve ser 1 ou 2")]
        public int QuantidadeLugares { get; set; } = 1;

        public bool Preferencial { get; set; } = false;

        public Sala? Sala { get; set; }

        public Ingresso? Ingresso { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public Assento() { }

        public Assento(char fila, int numero, Sala? sala = null, Ingresso? ingresso = null,
            TipoAssento tipo = TipoAssento.Normal, int quantidadeLugares = 1, bool preferencial = false)
        {
            Fila = fila;
            Numero = numero;
            Disponivel = true;
            Sala = sala;
            Ingresso = ingresso;
            Tipo = tipo;
            QuantidadeLugares = quantidadeLugares;
            Preferencial = preferencial;
            DataCriacao = DateTime.Now;
        }

        /// <summary>
        /// Marca o assento como disponível
        /// </summary>
        public void Liberar()
        {
            Disponivel = true;
        }

        /// <summary>
        /// Marca o assento como indisponível (reservado)
        /// </summary>
        public void Reservar()
        {
            Disponivel = false;
        }
    }
}
