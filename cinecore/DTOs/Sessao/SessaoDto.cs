using cinecore.enums;

namespace cinecore.DTOs.Sessao
{
    /// <summary>
    /// DTO para retorno de dados de Sessão
    /// </summary>
    public class SessaoDto
    {
        public int Id { get; set; }
        public DateTime DataHorario { get; set; }
        public decimal PrecoBase { get; set; }
        public decimal PrecoFinal { get; set; }
        public TipoSessao Tipo { get; set; }
        public string? NomeEvento { get; set; }
        public string? Parceiro { get; set; }
        public IdiomaSessao Idioma { get; set; }
        public int? FilmeId { get; set; }
        public int? SalaId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
