using cinecore.Enums;

namespace cinecore.DTOs.Relatorio
{
    /// <summary>
    /// DTO de retorno para sessoes em cartaz
    /// </summary>
    public class CartazSessaoDto
    {
        public int Id { get; set; }
        public DateTime DataHorario { get; set; }
        public decimal PrecoFinal { get; set; }
        public TipoSessao Tipo { get; set; }
        public IdiomaSessao Idioma { get; set; }
        public int SalaId { get; set; }
        public required string SalaNome { get; set; }
    }
}
