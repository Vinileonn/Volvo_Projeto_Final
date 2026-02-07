using cinema.modelos;
using cinema.servicos;
using cinema.excecoes;

namespace cinema.controladores
{
    // Renomeado de FilmeController para FilmeControlador.
    public class FilmeControlador
    {
        private readonly FilmeServico FilmeServico;

        public FilmeControlador(FilmeServico FilmeServico)
        {
            this.FilmeServico = FilmeServico;
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        public (bool sucesso, string mensagem) CriarFilme(Filme filme)
        {
            try
            {
                FilmeServico.CriarFilme(filme);
                return (true, $"Filme '{filme.Titulo}' cadastrado com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao cadastrar filme.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (Filme? filme, string mensagem) ObterFilme(int id)
        {
            try
            {
                var filme = FilmeServico.ObterFilme(id);
                return (filme, "Filme obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter filme.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<Filme> filmes, string mensagem) ListarFilmes()
        {
            try
            {
                var filmes = FilmeServico.ListarFilmes();
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
// ANTIGO: // LER - 
// LER - 
        public (List<Filme> filmes, string mensagem) BuscarPorTitulo(string titulo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titulo))
                {
                    return (new List<Filme>(), "Título não pode ser vazio.");
                }

                var filmes = FilmeServico.BuscarPorTitulo(titulo);
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
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
        public (bool sucesso, string mensagem) AtualizarFilme(int id, string? titulo = null, int? duracao = null,
                                   string? genero = null, DateTime? anoLancamento = null)
        {
            try
            {
                FilmeServico.AtualizarFilme(id, titulo, duracao, genero, anoLancamento);
                return (true, "Filme atualizado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao atualizar filme.");
            }
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
        public (bool sucesso, string mensagem) DeletarFilme(int id)
        {
            try
            {
                FilmeServico.DeletarFilme(id);
                return (true, "Filme deletado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
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





