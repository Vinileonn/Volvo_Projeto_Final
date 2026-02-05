using cinema.models;
using cinema.services;
using cinema.exceptions;

namespace cinema.controllers
{
    public class SessaoController
    {
        private readonly SessaoService sessaoService;

        public SessaoController(SessaoService sessaoService)
        {
            this.sessaoService = sessaoService;
        }

        // CREATE - cadastrar nova sessão
        public (bool sucesso, string mensagem) CriarSessao(Sessao sessao)
        {
            try
            {
                sessaoService.CriarSessao(sessao);
                return (true, "Sessão cadastrada com sucesso.");
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
                return (false, "Erro inesperado ao cadastrar sessão.");
            }
        }

        // READ - obter sessão por id
        public (Sessao? sessao, string mensagem) ObterSessao(int id)
        {
            try
            {
                var sessao = sessaoService.ObterSessao(id);
                return (sessao, "Sessão obtida com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter sessão.");
            }
        }

        // READ - listar todas as sessões
        public (List<Sessao> sessoes, string mensagem) ListarSessoes()
        {
            try
            {
                var sessoes = sessaoService.ListarSessoes();
                if (sessoes.Count == 0)
                {
                    return (sessoes, "Nenhuma sessão cadastrada.");
                }
                return (sessoes, $"{sessoes.Count} sessão(ões) encontrada(s).");
            }
            catch (Exception)
            {
                return (new List<Sessao>(), "Erro inesperado ao listar sessões.");
            }
        }

        // READ - listar sessões por filme
        public (List<Sessao> sessoes, string mensagem) ListarSessoesPorFilme(int filmeId)
        {
            try
            {
                var sessoes = sessaoService.ListarSessoesPorFilme(filmeId);
                if (sessoes.Count == 0)
                {
                    return (sessoes, $"Nenhuma sessão encontrada para o filme ID {filmeId}.");
                }
                return (sessoes, $"{sessoes.Count} sessão(ões) encontrada(s) para o filme.");
            }
            catch (Exception)
            {
                return (new List<Sessao>(), "Erro inesperado ao listar sessões por filme.");
            }
        }

        // READ - listar sessões por sala
        public (List<Sessao> sessoes, string mensagem) ListarSessoesPorSala(int salaId)
        {
            try
            {
                var sessoes = sessaoService.ListarSessoesPorSala(salaId);
                if (sessoes.Count == 0)
                {
                    return (sessoes, $"Nenhuma sessão encontrada para a sala ID {salaId}.");
                }
                return (sessoes, $"{sessoes.Count} sessão(ões) encontrada(s) para a sala.");
            }
            catch (Exception)
            {
                return (new List<Sessao>(), "Erro inesperado ao listar sessões por sala.");
            }
        }

        // UPDATE - atualizar sessão
        public (bool sucesso, string mensagem) AtualizarSessao(int id, DateTime? dataHorario = null, float? preco = null, Filme? filme = null, Sala? sala = null)
        {
            try
            {
                sessaoService.AtualizarSessao(id, dataHorario, preco, filme, sala);
                return (true, "Sessão atualizada com sucesso.");
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
                return (false, "Erro inesperado ao atualizar sessão.");
            }
        }

        // DELETE - remover sessão
        public (bool sucesso, string mensagem) DeletarSessao(int id)
        {
            try
            {
                sessaoService.DeletarSessao(id);
                return (true, "Sessão deletada com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao deletar sessão.");
            }
        }
    }
}
