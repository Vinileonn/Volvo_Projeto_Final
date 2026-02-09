using cinecore.DTOs.Filme;

namespace cinecore.DTOs.Relatorio
{
    /// <summary>
    /// DTO de retorno para filmes em cartaz com sessoes
    /// </summary>
    public class CartazFilmeDto
    {
        public required FilmeDto Filme { get; set; }
        public List<CartazSessaoDto> Sessoes { get; set; } = new List<CartazSessaoDto>();
    }
}
