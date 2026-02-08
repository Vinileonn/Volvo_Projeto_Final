using cineflow.modelos;
using cineflow.excecoes;

namespace cineflow.servicos
{
    public class CinemaServico
    {
        private readonly List<Cinema> cinemas;

        public CinemaServico()
        {
            cinemas = new List<Cinema>();
        }

        public void CriarCinema(Cinema cinema)
        {
            if (cinema == null)
            {
                throw new DadosInvalidosExcecao("Cinema nao pode ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(cinema.Nome) || string.IsNullOrWhiteSpace(cinema.Endereco))
            {
                throw new DadosInvalidosExcecao("Nome e endereco sao obrigatorios.");
            }

            if (cinemas.Any(c => c.Nome.Equals(cinema.Nome, StringComparison.OrdinalIgnoreCase)))
            {
                throw new OperacaoNaoPermitidaExcecao($"Cinema '{cinema.Nome}' ja existe.");
            }

            cinema.Id = cinemas.Count > 0 ? cinemas.Max(c => c.Id) + 1 : 1;
            cinemas.Add(cinema);
        }

        public Cinema ObterCinema(int id)
        {
            var cinema = cinemas.FirstOrDefault(c => c.Id == id);
            if (cinema == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Cinema com ID {id} nao encontrado.");
            }
            return cinema;
        }

        public List<Cinema> ListarCinemas()
        {
            return new List<Cinema>(cinemas);
        }

        public void AtualizarCinema(int id, string? nome = null, string? endereco = null)
        {
            var cinema = ObterCinema(id);

            if (!string.IsNullOrWhiteSpace(nome))
            {
                if (cinemas.Any(c => c.Id != id && c.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new OperacaoNaoPermitidaExcecao($"Ja existe um cinema com o nome '{nome}'.");
                }
                cinema.Nome = nome;
            }

            if (!string.IsNullOrWhiteSpace(endereco))
            {
                cinema.Endereco = endereco;
            }
        }

        public void DeletarCinema(int id)
        {
            var cinema = ObterCinema(id);
            cinemas.Remove(cinema);
        }
    }
}





