namespace cinecore.DTOs.Relatorio
{
    /// <summary>
    /// DTO de retorno para taxa de ocupacao por sala
    /// </summary>
    public class OcupacaoSalaDto
    {
        public int SalaId { get; set; }
        public required string SalaNome { get; set; }
        public int CapacidadeTotal { get; set; }
        public int IngressosVendidos { get; set; }
        public decimal TaxaOcupacao { get; set; }
    }
}
