namespace cinecore.DTOs.Cinema
{
    /// <summary>
    /// DTO para retorno de dados de Cinema
    /// </summary>
    public class CinemaDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Endereco { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
