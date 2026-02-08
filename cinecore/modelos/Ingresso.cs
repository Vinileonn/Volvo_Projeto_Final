using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados abstrato para representar um ingresso na WebAPI
    /// </summary>
    public abstract class Ingresso
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A fila é obrigatória")]
        public char Fila { get; set; }

        [Required(ErrorMessage = "O número é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O número deve ser maior que zero")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "A data de compra é obrigatória")]
        public DateTime DataCompra { get; set; } = DateTime.Now;

        public FormaPagamento? FormaPagamento { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O valor pago não pode ser negativo")]
        public decimal ValorPago { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "O valor do troco não pode ser negativo")]
        public decimal ValorTroco { get; set; } = 0;

        public bool ReservaAntecipada { get; set; } = false;

        [Range(0, float.MaxValue, ErrorMessage = "A taxa de reserva não pode ser negativa")]
        public float TaxaReserva { get; set; } = 0;

        public bool CheckInRealizado { get; set; } = false;

        public DateTime? DataCheckIn { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Os pontos usados não podem ser negativos")]
        public int PontosUsados { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Os pontos gerados não podem ser negativos")]
        public int PontosGerados { get; set; } = 0;

        public Sessao? Sessao { get; set; }

        public Cliente? Cliente { get; set; }

        public Assento? Assento { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public Ingresso() { }

        public Ingresso(int id, char fila, int numero, DateTime dataCompra, Sessao? sessao = null,
            Cliente? cliente = null, Assento? assento = null)
        {
            Id = id;
            Fila = fila;
            Numero = numero;
            DataCompra = dataCompra;
            Sessao = sessao;
            Cliente = cliente;
            Assento = assento;
            DataCriacao = DateTime.Now;
        }

        public abstract float CalcularPreco(float precoBase);
        public abstract string ObterTipo();
    }
}
