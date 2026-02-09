using cinecore.enums;

namespace cinecore.DTOs.Ingresso
{
    public class IngressoDto
    {
        public int Id { get; set; }
        public char Fila { get; set; }
        public int Numero { get; set; }
        public DateTime DataCompra { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorTroco { get; set; }
        public bool ReservaAntecipada { get; set; }
        public float TaxaReserva { get; set; }
        public bool CheckInRealizado { get; set; }
        public DateTime? DataCheckIn { get; set; }
        public int PontosUsados { get; set; }
        public int PontosGerados { get; set; }
        
        // Informações simplificadas da sessão
        public int SessaoId { get; set; }
        public DateTime SessaoDataHorario { get; set; }
        public string? FilmeTitulo { get; set; }
        public string? SalaNome { get; set; }
        public TipoSessao TipoSessao { get; set; }
        
        // Informações do cliente
        public int ClienteId { get; set; }
        public string? ClienteNome { get; set; }
        
        // Informações do assento
        public TipoAssento TipoAssento { get; set; }
        public bool AssentoPreferencial { get; set; }
    }
}
