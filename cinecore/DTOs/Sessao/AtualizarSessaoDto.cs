using cinecore.enums;

namespace cinecore.DTOs.Sessao
{
    /// <summary>
    /// DTO para atualização de uma Sessão existente
    /// </summary>
    public class AtualizarSessaoDto
    {
        public DateTime? DataHorario { get; set; }
        public float? PrecoBase { get; set; }
        public int? FilmeId { get; set; }
        public int? SalaId { get; set; }
        public TipoSessao? Tipo { get; set; }
        public string? NomeEvento { get; set; }
        public string? Parceiro { get; set; }
        public IdiomaSessao? Idioma { get; set; }
    }
}
