using cineflow.modelos;
using cineflow.enumeracoes;
using cineflow.excecoes;

namespace cineflow.servicos
{
    public class LimpezaServico
    {
        private readonly List<EscalaLimpeza> escalas;

        public LimpezaServico()
        {
            escalas = new List<EscalaLimpeza>();
        }

        public void CriarEscala(Sala sala, Funcionario funcionario, DateTime inicio, DateTime fim)
        {
            if (sala == null || funcionario == null)
            {
                throw new DadosInvalidosExcecao("Sala e funcionario sao obrigatorios.");
            }

            if (funcionario.Cargo != CargoFuncionario.Limpeza)
            {
                throw new OperacaoNaoPermitidaExcecao("Funcionario nao e da limpeza.");
            }

            if (inicio == default || fim == default || inicio >= fim)
            {
                throw new DadosInvalidosExcecao("Periodo de limpeza invalido.");
            }

            // Evita conflito de horarios na mesma sala ou para o mesmo funcionario.
            if (escalas.Any(e =>
                (e.Sala.Id == sala.Id || e.Funcionario.Id == funcionario.Id) &&
                inicio < e.Fim && fim > e.Inicio))
            {
                throw new OperacaoNaoPermitidaExcecao("Conflito de horario na escala de limpeza.");
            }

            var escala = new EscalaLimpeza(ProximoId(), sala, funcionario, inicio, fim);
            escalas.Add(escala);
        }

        public List<EscalaLimpeza> ListarEscalas()
        {
            return new List<EscalaLimpeza>(escalas);
        }

        public List<EscalaLimpeza> ListarPorSala(int salaId)
        {
            return escalas.Where(e => e.Sala.Id == salaId).ToList();
        }

        public List<EscalaLimpeza> ListarPorFuncionario(int funcionarioId)
        {
            return escalas.Where(e => e.Funcionario.Id == funcionarioId).ToList();
        }

        public void DeletarEscala(int id)
        {
            var escala = escalas.FirstOrDefault(e => e.Id == id);
            if (escala == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Escala com ID {id} nao encontrada.");
            }

            escalas.Remove(escala);
        }

        private int ProximoId()
        {
            return escalas.Count > 0 ? escalas.Max(e => e.Id) + 1 : 1;
        }
    }
}





