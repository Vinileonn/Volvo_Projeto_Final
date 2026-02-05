using cinema.models;
using cinema.services;
using cinema.exceptions;

namespace cinema.controllers
{
    public class FilmeController
    {
        private readonly FilmeService filmeService;

        public FilmeController(FilmeService filmeService)
        {
            this.filmeService = filmeService;
        }

        // CREATE - cadastrar novo filme
        public (bool sucesso, string mensagem) CriarFilme(Filme filme)
        {
            try
            {
                filmeService.CriarFilme(filme);
                return (true, $"Filme '{filme.Titulo}' cadastrado com sucesso.");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao cadastrar filme.");
            }
        }

        // READ - obter filme por id
        public (Filme? filme, string mensagem) ObterFilme(int id)
        {
            try
            {
                var filme = filmeService.ObterFilme(id);
                return (filme, "Filme obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter filme.");
            }
        }

        // READ - listar todos os filmes
        public (List<Filme> filmes, string mensagem) ListarFilmes()
        {
            try
            {
                var filmes = filmeService.ListarFilmes();
                if (filmes.Count == 0)
                {
                    return (filmes, "Nenhum filme cadastrado.");
                }
                return (filmes, $"{filmes.Count} filme(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<Filme>(), "Erro inesperado ao listar filmes.");
            }
        }

        // READ - buscar filmes por título
        public (List<Filme> filmes, string mensagem) BuscarPorTitulo(string titulo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titulo))
                {
                    return (new List<Filme>(), "Título não pode ser vazio.");
                }

                var filmes = filmeService.BuscarPorTitulo(titulo);
                if (filmes.Count == 0)
                {
                    return (filmes, $"Nenhum filme encontrado com o título '{titulo}'.");
                }
                return (filmes, $"{filmes.Count} filme(s) encontrado(s) com o título '{titulo}'.");
            }
            catch (Exception)
            {
                return (new List<Filme>(), "Erro inesperado ao buscar filmes.");
            }
        }

        // UPDATE - atualizar dados do filme
        public (bool sucesso, string mensagem) AtualizarFilme(int id, string? titulo = null, int? duracao = null,
                                   string? genero = null, DateTime? anoLancamento = null)
        {
            try
            {
                filmeService.AtualizarFilme(id, titulo, duracao, genero, anoLancamento);
                return (true, "Filme atualizado com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao atualizar filme.");
            }
        }

        // DELETE - remover filme
        public (bool sucesso, string mensagem) DeletarFilme(int id)
        {
            try
            {
                filmeService.DeletarFilme(id);
                return (true, "Filme deletado com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao deletar filme.");
            }
        }
    }
}
