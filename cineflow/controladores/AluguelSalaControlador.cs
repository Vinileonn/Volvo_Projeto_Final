using cineflow.enumeracoes;
using cineflow.excecoes;
using cineflow.modelos;
using cineflow.modelos.UsuarioModelo;
using cineflow.servicos;

namespace cineflow.controladores
{
    public class AluguelSalaControlador
    {
        private readonly AluguelSalaServico aluguelServico;

        public AluguelSalaControlador(AluguelSalaServico aluguelServico)
        {
            this.aluguelServico = aluguelServico;
        }

        public (bool sucesso, string mensagem) SolicitarAluguel(AluguelSala aluguel)
        {
            try
            {
                aluguelServico.SolicitarAluguel(aluguel);
                return (true, "Aluguel solicitado com sucesso.");
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
                return (false, "Erro inesperado ao solicitar aluguel.");
            }
        }

        public (AluguelSala? aluguel, string mensagem) ObterAluguel(int id)
        {
            try
            {
                var aluguel = aluguelServico.ObterAluguel(id);
                return (aluguel, "Aluguel obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter aluguel.");
            }
        }

        public (List<AluguelSala> alugueis, string mensagem) ListarAlugueis()
        {
            try
            {
                var alugueis = aluguelServico.ListarAlugueis();
                if (alugueis.Count == 0)
                {
                    return (alugueis, "Nenhum aluguel cadastrado.");
                }
                return (alugueis, $"{alugueis.Count} aluguel(is) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<AluguelSala>(), "Erro inesperado ao listar alugueis.");
            }
        }

        public (List<AluguelSala> alugueis, string mensagem) ListarPorStatus(StatusAluguel status)
        {
            try
            {
                var alugueis = aluguelServico.ListarPorStatus(status);
                if (alugueis.Count == 0)
                {
                    return (alugueis, "Nenhum aluguel encontrado para este status.");
                }
                return (alugueis, $"{alugueis.Count} aluguel(is) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<AluguelSala>(), "Erro inesperado ao listar alugueis por status.");
            }
        }

        public (List<AluguelSala> alugueis, string mensagem) ListarPorCliente(Cliente cliente)
        {
            try
            {
                var alugueis = aluguelServico.ListarPorCliente(cliente);
                if (alugueis.Count == 0)
                {
                    return (alugueis, "Nenhum aluguel encontrado para este cliente.");
                }
                return (alugueis, $"{alugueis.Count} aluguel(is) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<AluguelSala>(), "Erro inesperado ao listar alugueis do cliente.");
            }
        }

        public (bool sucesso, string mensagem) AprovarAluguel(int id, decimal? valor = null)
        {
            try
            {
                aluguelServico.AprovarAluguel(id, valor);
                return (true, "Aluguel aprovado com sucesso.");
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
                return (false, "Erro inesperado ao aprovar aluguel.");
            }
        }

        public (bool sucesso, string mensagem) CancelarAluguel(int id)
        {
            try
            {
                aluguelServico.CancelarAluguel(id);
                return (true, "Aluguel cancelado com sucesso.");
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
                return (false, "Erro inesperado ao cancelar aluguel.");
            }
        }
    }
}
