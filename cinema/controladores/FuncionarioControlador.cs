using cinema.modelos;
using cinema.enumeracoes;
using cinema.servicos;
using cinema.excecoes;

namespace cinema.controladores
{
    // Renomeado de FuncionarioController para FuncionarioControlador.
    public class FuncionarioControlador
    {
        private readonly FuncionarioServico FuncionarioServico;

        public FuncionarioControlador(FuncionarioServico FuncionarioServico)
        {
            this.FuncionarioServico = FuncionarioServico;
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        public (bool sucesso, string mensagem) CriarFuncionario(Funcionario funcionario)
        {
            try
            {
                FuncionarioServico.CriarFuncionario(funcionario);
                return (true, "Funcionario cadastrado com sucesso.");
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
                return (false, "Erro inesperado ao cadastrar funcionario.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (Funcionario? funcionario, string mensagem) ObterFuncionario(int id)
        {
            try
            {
                var funcionario = FuncionarioServico.ObterFuncionario(id);
                return (funcionario, "Funcionario obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter funcionario.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<Funcionario> funcionarios, string mensagem) ListarFuncionarios()
        {
            try
            {
                var funcionarios = FuncionarioServico.ListarFuncionarios();
                if (funcionarios.Count == 0)
                {
                    return (funcionarios, "Nenhum funcionario cadastrado.");
                }
                return (funcionarios, $"{funcionarios.Count} funcionario(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<Funcionario>(), "Erro inesperado ao listar funcionarios.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<Funcionario> funcionarios, string mensagem) ListarPorCargo(CargoFuncionario cargo)
        {
            try
            {
                var funcionarios = FuncionarioServico.ListarPorCargo(cargo);
                if (funcionarios.Count == 0)
                {
                    return (funcionarios, "Nenhum funcionario encontrado para este cargo.");
                }
                return (funcionarios, $"{funcionarios.Count} funcionario(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<Funcionario>(), "Erro inesperado ao listar funcionarios por cargo.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<Funcionario> funcionarios, string mensagem) ListarPorCinema(int cinemaId)
        {
            try
            {
                var funcionarios = FuncionarioServico.ListarPorCinema(cinemaId);
                if (funcionarios.Count == 0)
                {
                    return (funcionarios, "Nenhum funcionario encontrado para este cinema.");
                }
                return (funcionarios, $"{funcionarios.Count} funcionario(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<Funcionario>(), "Erro inesperado ao listar funcionarios por cinema.");
            }
        }
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
        public (bool sucesso, string mensagem) AtualizarFuncionario(int id, string? nome = null, CargoFuncionario? cargo = null, Cinema? cinema = null)
        {
            try
            {
                FuncionarioServico.AtualizarFuncionario(id, nome, cargo, cinema);
                return (true, "Funcionario atualizado com sucesso.");
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
                return (false, "Erro inesperado ao atualizar funcionario.");
            }
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
        public (bool sucesso, string mensagem) DeletarFuncionario(int id)
        {
            try
            {
                FuncionarioServico.DeletarFuncionario(id);
                return (true, "Funcionario deletado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao deletar funcionario.");
            }
        }
    }
}





