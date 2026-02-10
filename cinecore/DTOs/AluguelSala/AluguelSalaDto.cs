using cinecore.Enums;

namespace cinecore.DTOs.AluguelSala
{
    /// <summary>
    /// DTO para retorno de dados de Aluguel de Sala
    /// </summary>
    public class AluguelSalaDto
    {
        public int Id { get; set; }
        public required string NomeCliente { get; set; }
        public required string Contato { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public required string Motivo { get; set; }
        public decimal Valor { get; set; }
        public StatusAluguel Status { get; set; }
        public bool PacoteAniversario { get; set; }
        public int SalaId { get; set; }
        public int? ClienteId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
