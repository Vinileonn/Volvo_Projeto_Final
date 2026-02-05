using cinema.models;
using cinema.exceptions;

namespace cinema.services
{
    public class FilmeService
    {
        private readonly List<Filme> filmes;

        public FilmeService()
        {
            filmes = new List<Filme>();
        }

        // CREATE - cadastrar novo filme
        public void CriarFilme(Filme filme)
        {
            if (filme == null)
            {
                throw new DadosInvalidosException("Filme não pode ser nulo.");
            }

            // Valida campos obrigatórios juntos
            var camposVazios = new List<string>();
            if (string.IsNullOrWhiteSpace(filme.Titulo))
                camposVazios.Add("título");
            if (string.IsNullOrWhiteSpace(filme.Genero))
                camposVazios.Add("gênero");
            if (filme.AnoLancamento == default)
                camposVazios.Add("ano de lançamento");
            
            if (camposVazios.Count > 0)
            {
                throw new DadosInvalidosException($"Campos obrigatórios faltando: {string.Join(", ", camposVazios)}.");
            }

            if (filme.Duracao <= 0)
            {
                throw new DadosInvalidosException("Duração inválida.");
            }

            // Evita duplicidade simples (título + ano)
            if (filmes.Any(f =>
                f.Titulo.Equals(filme.Titulo, StringComparison.OrdinalIgnoreCase) &&
                f.AnoLancamento.Year == filme.AnoLancamento.Year))
            {
                throw new OperacaoNaoPermitidaException($"Filme '{filme.Titulo}' ({filme.AnoLancamento.Year}) já existe.");
            }

            filme.Id = filmes.Count > 0 ? filmes.Max(f => f.Id) + 1 : 1;
            filmes.Add(filme);
        }

        // READ - obter filme por id
        public Filme ObterFilme(int id)
        {
            var filme = filmes.FirstOrDefault(f => f.Id == id);
            if (filme == null)
            {
                throw new RecursoNaoEncontradoException($"Filme com ID {id} não encontrado.");
            }
            return filme;
        }

        // READ - listar todos os filmes
        public List<Filme> ListarFilmes()
        {
            return new List<Filme>(filmes);
        }

        // READ - buscar por título
        public List<Filme> BuscarPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return new List<Filme>();
            }

            return filmes
                .Where(f => f.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // UPDATE - atualizar dados do filme
        public void AtualizarFilme(int id, string? titulo = null, int? duracao = null,
                                   string? genero = null, DateTime? anoLancamento = null)
        {
            var filme = ObterFilme(id);

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                // Evita conflito com outro filme (título + ano)
                if (filmes.Any(f => f.Id != id &&
                    f.Titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase) &&
                    (anoLancamento?.Year ?? filme.AnoLancamento.Year) == f.AnoLancamento.Year))
                {
                    throw new OperacaoNaoPermitidaException($"Já existe um filme com título '{titulo}' do ano {anoLancamento?.Year ?? filme.AnoLancamento.Year}.");
                }
            }

            if (duracao.HasValue && duracao.Value <= 0)
            {
                throw new DadosInvalidosException("Duração deve ser maior que zero.");
            }

            if (anoLancamento.HasValue && anoLancamento.Value == default)
            {
                throw new DadosInvalidosException("Ano de lançamento inválido.");
            }

            filme.AtualizarDetalhes(titulo, duracao, genero, anoLancamento);
        }

        // DELETE - remover filme
        public void DeletarFilme(int id)
        {
            var filme = ObterFilme(id);
            filmes.Remove(filme);
        }
    }
}