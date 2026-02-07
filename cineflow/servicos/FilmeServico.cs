using cineflow.modelos;
using cineflow.excecoes;

namespace cineflow.servicos
{
    // Renomeado de FilmeService para FilmeServico.
    public class FilmeServico
    {
        private readonly List<Filme> filmes;

        public FilmeServico()
        {
            filmes = new List<Filme>();
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        public void CriarFilme(Filme filme)
        {
            if (filme == null)
            {
                throw new DadosInvalidosExcecao("Filme não pode ser nulo.");
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
                throw new DadosInvalidosExcecao($"Campos obrigatórios faltando: {string.Join(", ", camposVazios)}.");
            }

            if (filme.Duracao <= 0)
            {
                throw new DadosInvalidosExcecao("Duração inválida.");
            }

            // Evita duplicidade simples (título + ano)
            if (filmes.Any(f =>
                f.Titulo.Equals(filme.Titulo, StringComparison.OrdinalIgnoreCase) &&
                f.AnoLancamento.Year == filme.AnoLancamento.Year))
            {
                throw new OperacaoNaoPermitidaExcecao($"Filme '{filme.Titulo}' ({filme.AnoLancamento.Year}) já existe.");
            }

            filme.Id = filmes.Count > 0 ? filmes.Max(f => f.Id) + 1 : 1;
            filmes.Add(filme);
        }
// ANTIGO: // LER - 
// LER - 
        public Filme ObterFilme(int id)
        {
            var filme = filmes.FirstOrDefault(f => f.Id == id);
            if (filme == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Filme com ID {id} não encontrado.");
            }
            return filme;
        }
// ANTIGO: // LER - 
// LER - 
        public List<Filme> ListarFilmes()
        {
            return new List<Filme>(filmes);
        }
// ANTIGO: // LER - 
// LER - 
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
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
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
                    throw new OperacaoNaoPermitidaExcecao($"Já existe um filme com título '{titulo}' do ano {anoLancamento?.Year ?? filme.AnoLancamento.Year}.");
                }
            }

            if (duracao.HasValue && duracao.Value <= 0)
            {
                throw new DadosInvalidosExcecao("Duração deve ser maior que zero.");
            }

            if (anoLancamento.HasValue && anoLancamento.Value == default)
            {
                throw new DadosInvalidosExcecao("Ano de lançamento inválido.");
            }

            filme.AtualizarDetalhes(titulo, duracao, genero, anoLancamento);
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
        public void DeletarFilme(int id)
        {
            var filme = ObterFilme(id);
            filmes.Remove(filme);
        }
    }
}




