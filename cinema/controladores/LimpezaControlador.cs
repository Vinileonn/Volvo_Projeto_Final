using cinema.modelos;
using cinema.servicos;
using cinema.excecoes;

namespace cinema.controladores
{
    // Renomeado de LimpezaController para LimpezaControlador.
    public class LimpezaControlador
    {
        private readonly LimpezaServico LimpezaServico;

        public LimpezaControlador(LimpezaServico LimpezaServico)
        {
            this.LimpezaServico = LimpezaServico;
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        public (bool sucesso, string mensagem) CriarEscala(Sala sala, Funcionario funcionario, DateTime inicio, DateTime fim)
        {
            try
            {
                LimpezaServico.CriarEscala(sala, funcionario, inicio, fim);
                return (true, "Escala de limpeza criada com sucesso.");
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
                return (false, "Erro inesperado ao criar escala.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<EscalaLimpeza> escalas, string mensagem) ListarEscalas()
        {
            try
            {
                var escalas = LimpezaServico.ListarEscalas();
                if (escalas.Count == 0)
                {
                    return (escalas, "Nenhuma escala cadastrada.");
                }
                return (escalas, $"{escalas.Count} escala(s) encontrada(s).");
            }
            catch (Exception)
            {
                return (new List<EscalaLimpeza>(), "Erro inesperado ao listar escalas.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<EscalaLimpeza> escalas, string mensagem) ListarPorSala(int salaId)
        {
            try
            {
                var escalas = LimpezaServico.ListarPorSala(salaId);
                if (escalas.Count == 0)
                {
                    return (escalas, "Nenhuma escala para esta sala.");
                }
                return (escalas, $"{escalas.Count} escala(s) encontrada(s).");
            }
            catch (Exception)
            {
                return (new List<EscalaLimpeza>(), "Erro inesperado ao listar escalas por sala.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<EscalaLimpeza> escalas, string mensagem) ListarPorFuncionario(int funcionarioId)
        {
            try
            {
                var escalas = LimpezaServico.ListarPorFuncionario(funcionarioId);
                if (escalas.Count == 0)
                {
                    return (escalas, "Nenhuma escala para este funcionario.");
                }
                return (escalas, $"{escalas.Count} escala(s) encontrada(s).");
            }
            catch (Exception)
            {
                return (new List<EscalaLimpeza>(), "Erro inesperado ao listar escalas por funcionario.");
            }
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
        public (bool sucesso, string mensagem) DeletarEscala(int id)
        {
            try
            {
                LimpezaServico.DeletarEscala(id);
                return (true, "Escala removida com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao remover escala.");
            }
        }
    }
}





