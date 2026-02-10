using System.ComponentModel.DataAnnotations;
using cinecore.Enums;

namespace cinecore.Models
{
    /// <summary>
    /// Modelo de dados para representar um aluguel de sala na WebAPI
    /// </summary>
    public class AluguelSala
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome do cliente deve ter entre 1 e 200 caracteres")]
        public required string NomeCliente { get; set; }

        [Required(ErrorMessage = "O contato é obrigatório")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O contato deve ter entre 1 e 100 caracteres")]
        public required string Contato { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória")]
        public DateTime Inicio { get; set; }

        [Required(ErrorMessage = "A data de fim é obrigatória")]
        public DateTime Fim { get; set; }

        [Required(ErrorMessage = "O motivo é obrigatório")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O motivo deve ter entre 1 e 500 caracteres")]
        public required string Motivo { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor não pode ser negativo")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        public StatusAluguel Status { get; set; } = StatusAluguel.Solicitado;

        public bool PacoteAniversario { get; set; } = false;

        public Sala? Sala { get; set; }

        public Cliente? Cliente { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public AluguelSala() { }

        public AluguelSala(int id, DateTime inicio, DateTime fim, string nomeCliente, string contato,
            string motivo, decimal valor, Sala? sala = null, Cliente? cliente = null,
            StatusAluguel status = StatusAluguel.Solicitado, bool pacoteAniversario = false)
        {
            Id = id;
            Inicio = inicio;
            Fim = fim;
            NomeCliente = nomeCliente;
            Contato = contato;
            Motivo = motivo;
            Valor = valor;
            Sala = sala;
            Cliente = cliente;
            Status = status;
            PacoteAniversario = pacoteAniversario;
            DataCriacao = DateTime.Now;
        }
    }
}
