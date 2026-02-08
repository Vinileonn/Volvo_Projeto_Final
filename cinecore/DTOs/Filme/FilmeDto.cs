using cinecore.enums;

namespace cinecore.DTOs.Filme
{
    /// <summary>
    /// DTO para retorno de dados de Filme
    /// </summary>
    public class FilmeDto
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public int Duracao { get; set; }
        public required string Genero { get; set; }
        public DateTime AnoLancamento { get; set; }
        public bool Eh3D { get; set; }
        public ClassificacaoIndicativa Classificacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public int QuantidadeSessoes { get; set; }
    }
}
