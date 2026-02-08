using cineflow.modelos;
using cineflow.servicos;
using cineflow.excecoes;
using cineflow.enumeracoes;

namespace cineflow.controladores
{
    public class SessaoControlador
    {
        private readonly SessaoServico SessaoServico;

        public SessaoControlador(SessaoServico SessaoServico)
        {
            this.SessaoServico = SessaoServico;
        }
        public (bool sucesso, string mensagem) CriarSessao(Sessao sessao)
        {
            try
            {
                SessaoServico.CriarSessao(sessao);
                return (true, "Sessão cadastrada com sucesso.");
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
                return (false, "Erro inesperado ao cadastrar sessão.");
            }
        }
        public (Sessao? sessao, string mensagem) ObterSessao(int id)
        {
            try
            {
                var sessao = SessaoServico.ObterSessao(id);
                return (sessao, "Sessão obtida com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter sessão.");
            }
        }
        public (List<Sessao> sessoes, string mensagem) ListarSessoes()
        {
            try
            {
                var sessoes = SessaoServico.ListarSessoes();
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
        public (List<Sessao> sessoes, string mensagem) ListarSessoesPorFilme(int filmeId)
        {
            try
            {
                var sessoes = SessaoServico.ListarSessoesPorFilme(filmeId);
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
        public (List<Sessao> sessoes, string mensagem) ListarSessoesPorSala(int salaId)
        {
            try
            {
                var sessoes = SessaoServico.ListarSessoesPorSala(salaId);
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

        public (bool sucesso, string mensagem) AtualizarSessao(int id, DateTime? dataHorario = null, float? precoBase = null,
            Filme? filme = null, Sala? sala = null, TipoSessao? tipo = null, string? nomeEvento = null, string? parceiro = null,
            IdiomaSessao? idioma = null)
        {
            try
            {
                SessaoServico.AtualizarSessao(id, dataHorario, precoBase, filme, sala, tipo, nomeEvento, parceiro, idioma);
                return (true, "Sessao atualizada com sucesso.");
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
                return (false, "Erro inesperado ao atualizar sessão.");
            }
        }
        public (bool sucesso, string mensagem) DeletarSessao(int id)
        {
            try
            {
                SessaoServico.DeletarSessao(id);
                return (true, "Sessão deletada com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
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





