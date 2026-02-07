using cineflow.modelos;
using cineflow.servicos;
using cineflow.excecoes;

namespace cineflow.controladores
{
    // Renomeado de SessaoController para SessaoControlador.
    public class SessaoControlador
    {
        private readonly SessaoServico SessaoServico;

        public SessaoControlador(SessaoServico SessaoServico)
        {
            this.SessaoServico = SessaoServico;
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
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
// ANTIGO: // LER - 
// LER - 
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
// ANTIGO: // LER - 
// LER - 
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
// ANTIGO: // LER - 
// LER - 
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
// ANTIGO: // LER - 
// LER - 
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
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
        // ANTIGO:
        // public (bool sucesso, string mensagem) AtualizarSessao(int id, DateTime? dataHorario = null, float? preco = null, Filme? filme = null, Sala? sala = null)
        // {
        //     try
        //     {
        //         SessaoServico.AtualizarSessao(id, dataHorario, preco, filme, sala);
        //         return (true, "Sessão atualizada com sucesso.");
        //     }
        //     catch (RecursoNaoEncontradoExcecao ex)
        //     {
        //         return (false, $"Recurso não encontrado: {ex.Message}");
        //     }
        //     catch (DadosInvalidosExcecao ex)
        //     {
        //         return (false, $"Dados inválidos: {ex.Message}");
        //     }
        //     catch (OperacaoNaoPermitidaExcecao ex)
        //     {
        //         return (false, $"Operação não permitida: {ex.Message}");
        //     }
        //     catch (Exception)
        //     {
        //         return (false, "Erro inesperado ao atualizar sessão.");
        //     }
        // }

        public (bool sucesso, string mensagem) AtualizarSessao(int id, DateTime? dataHorario = null, float? precoBase = null, Filme? filme = null, Sala? sala = null)
        {
            try
            {
                SessaoServico.AtualizarSessao(id, dataHorario, precoBase, filme, sala);
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
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
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





