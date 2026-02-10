using cinecore.enums;

namespace cinecore.DTOs.Sessao
{
    /// <summary>
    /// DTO para criação de uma nova Sessão
    /// </summary>
    public class CriarSessaoDto
    {
        public required DateTime DataHorario { get; set; }
        public required decimal PrecoBase { get; set; }
        public required int FilmeId { get; set; }
        public required int SalaId { get; set; }
        public TipoSessao Tipo { get; set; } = TipoSessao.Regular;
        public string? NomeEvento { get; set; }
        public string? Parceiro { get; set; }
        public IdiomaSessao Idioma { get; set; } = IdiomaSessao.Dublado;
    }
}
