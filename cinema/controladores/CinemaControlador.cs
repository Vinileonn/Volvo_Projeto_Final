using cinema.modelos;
using cinema.servicos;
using cinema.excecoes;

namespace cinema.controladores
{
    // Renomeado de CinemaController para CinemaControlador.
    public class CinemaControlador
    {
        private readonly CinemaServico CinemaServico;

        public CinemaControlador(CinemaServico CinemaServico)
        {
            this.CinemaServico = CinemaServico;
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        public (bool sucesso, string mensagem) CriarCinema(Cinema cinema)
        {
            try
            {
                CinemaServico.CriarCinema(cinema);
                return (true, $"Cinema '{cinema.Nome}' cadastrado com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados invalidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operacao nao permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao cadastrar cinema.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (Cinema? cinema, string mensagem) ObterCinema(int id)
        {
            try
            {
                var cinema = CinemaServico.ObterCinema(id);
                return (cinema, "Cinema obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter cinema.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<Cinema> cinemas, string mensagem) ListarCinemas()
        {
            try
            {
                var cinemas = CinemaServico.ListarCinemas();
                if (cinemas.Count == 0)
                {
                    return (cinemas, "Nenhum cinema cadastrado.");
                }
                return (cinemas, $"{cinemas.Count} cinema(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<Cinema>(), "Erro inesperado ao listar cinemas.");
            }
        }
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
        public (bool sucesso, string mensagem) AtualizarCinema(int id, string? nome = null, string? endereco = null)
        {
            try
            {
                CinemaServico.AtualizarCinema(id, nome, endereco);
                return (true, "Cinema atualizado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operacao nao permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao atualizar cinema.");
            }
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
        public (bool sucesso, string mensagem) DeletarCinema(int id)
        {
            try
            {
                CinemaServico.DeletarCinema(id);
                return (true, "Cinema deletado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao deletar cinema.");
            }
        }
    }
}





