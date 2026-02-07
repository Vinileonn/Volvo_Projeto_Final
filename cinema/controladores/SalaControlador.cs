using cinema.modelos;
using cinema.servicos;
using cinema.excecoes;

namespace cinema.controladores
{
    // Renomeado de SalaController para SalaControlador.
    public class SalaControlador
    {
        private readonly SalaServico SalaServico;

        public SalaControlador(SalaServico SalaServico)
        {
            this.SalaServico = SalaServico;
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        public (bool sucesso, string mensagem) CriarSala(Sala sala)
        {
            try
            {
                SalaServico.CriarSala(sala);
                return (true, $"Sala '{sala.Nome}' cadastrada com sucesso com {sala.Assentos.Count} assentos.");
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
                return (false, "Erro inesperado ao cadastrar sala.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (Sala? sala, string mensagem) ObterSala(int id)
        {
            try
            {
                var sala = SalaServico.ObterSala(id);
                return (sala, "Sala obtida com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter sala.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<Sala> salas, string mensagem) ListarSalas()
        {
            try
            {
                var salas = SalaServico.ListarSalas();
                if (salas.Count == 0)
                {
                    return (salas, "Nenhuma sala cadastrada.");
                }
                return (salas, $"{salas.Count} sala(s) encontrada(s).");
            }
            catch (Exception)
            {
                return (new List<Sala>(), "Erro inesperado ao listar salas.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<Sala> salas, string mensagem) ListarSalasPorCinema(int cinemaId)
        {
            try
            {
                var salas = SalaServico.ListarSalasPorCinema(cinemaId);
                if (salas.Count == 0)
                {
                    return (salas, "Nenhuma sala cadastrada para este cinema.");
                }
                return (salas, $"{salas.Count} sala(s) encontrada(s) para este cinema.");
            }
            catch (Exception)
            {
                return (new List<Sala>(), "Erro inesperado ao listar salas por cinema.");
            }
        }
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
        // ANTIGO:
        // public (bool sucesso, string mensagem) AtualizarSala(int id, string? nome = null, int? capacidade = null)
        // {
        //     try
        //     {
        //         SalaServico.AtualizarSala(id, nome, capacidade);
        //         return (true, "Sala atualizada com sucesso.");
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
        //         return (false, "Erro inesperado ao atualizar sala.");
        //     }
        // }

        public (bool sucesso, string mensagem) AtualizarSala(int id, string? nome = null, int? capacidade = null,
            int? quantidadeAssentosCasal = null, int? quantidadeAssentosPCD = null)
        {
            try
            {
                SalaServico.AtualizarSala(id, nome, capacidade, quantidadeAssentosCasal, quantidadeAssentosPCD);
                return (true, "Sala atualizada com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso nao encontrado: {ex.Message}");
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
                return (false, "Erro inesperado ao atualizar sala.");
            }
        }

        // UTIL - gerar assentos para sala
        public (bool sucesso, string mensagem) GerarAssentosParaSala(int id)
        {
            try
            {
                SalaServico.GerarAssentosParaSala(id);
                return (true, "Assentos gerados com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao gerar assentos.");
            }
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
        public (bool sucesso, string mensagem) DeletarSala(int id)
        {
            try
            {
                SalaServico.DeletarSala(id);
                return (true, "Sala deletada com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao deletar sala.");
            }
        }
    }
}





