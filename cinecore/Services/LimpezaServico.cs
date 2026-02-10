using cinecore.Data;
using cinecore.Models;
using cinecore.Enums;
using cinecore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Services
{
    public class LimpezaServico
    {
        private readonly CineFlowContext _context;

        public LimpezaServico(CineFlowContext context)
        {
            _context = context;
        }

        public EscalaLimpeza CriarEscala(Sala sala, Funcionario funcionario, DateTime inicio, DateTime fim)
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
            if (_context.EscalasLimpeza.Any(e =>
                e.Sala != null && e.Funcionario != null &&
                (e.Sala.Id == sala.Id || e.Funcionario.Id == funcionario.Id) &&
                inicio < e.Fim && fim > e.Inicio))
            {
                throw new OperacaoNaoPermitidaExcecao("Conflito de horario na escala de limpeza.");
            }

            var escala = new EscalaLimpeza(0, inicio, fim, sala, funcionario);
            _context.EscalasLimpeza.Add(escala);
            _context.SaveChanges();
            return escala;
        }

        public List<EscalaLimpeza> ListarEscalas()
        {
            return _context.EscalasLimpeza
                .Include(e => e.Sala)
                .Include(e => e.Funcionario)
                .ToList();
        }

        public List<EscalaLimpeza> ListarPorSala(int salaId)
        {
            return _context.EscalasLimpeza
                .Include(e => e.Sala)
                .Include(e => e.Funcionario)
                .Where(e => e.Sala != null && e.Sala.Id == salaId)
                .ToList();
        }

        public List<EscalaLimpeza> ListarPorFuncionario(int funcionarioId)
        {
            return _context.EscalasLimpeza
                .Include(e => e.Sala)
                .Include(e => e.Funcionario)
                .Where(e => e.Funcionario != null && e.Funcionario.Id == funcionarioId)
                .ToList();
        }

        public EscalaLimpeza DeletarEscala(int id)
        {
            var escala = _context.EscalasLimpeza.FirstOrDefault(e => e.Id == id);
            if (escala == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Escala com ID {id} nao encontrada.");
            }

            _context.EscalasLimpeza.Remove(escala);
            _context.SaveChanges();
            return escala;
        }
    }
}
