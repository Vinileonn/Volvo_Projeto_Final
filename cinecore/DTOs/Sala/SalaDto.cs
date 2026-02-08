using cinecore.enums;

namespace cinecore.DTOs.Sala
{
    /// <summary>
    /// DTO para retorno de dados de Sala
    /// </summary>
    public class SalaDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public int Capacidade { get; set; }
        public TipoSala Tipo { get; set; }
        public int QuantidadeAssentosCasal { get; set; }
        public int QuantidadeAssentosPCD { get; set; }
        public int CinemaId { get; set; }
        public int QuantidadeAssentos { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
