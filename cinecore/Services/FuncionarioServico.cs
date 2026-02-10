using cinecore.Data;
using cinecore.Models;
using cinecore.Enums;
using cinecore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Services
{
    public class FuncionarioServico
    {
        private readonly CineFlowContext _context;

        public FuncionarioServico(CineFlowContext context)
        {
            _context = context;
        }

        public void CriarFuncionario(Funcionario funcionario)
        {
            if (funcionario == null)
            {
                throw new DadosInvalidosExcecao("Funcionario nao pode ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(funcionario.Nome))
            {
                throw new DadosInvalidosExcecao("Nome do funcionario e obrigatorio.");
            }

            if (funcionario.Cinema == null)
            {
                throw new DadosInvalidosExcecao("Cinema do funcionario e obrigatorio.");
            }
            funcionario.DataCriacao = DateTime.Now;
            _context.Funcionarios.Add(funcionario);
            _context.SaveChanges();
        }

        public Funcionario ObterFuncionario(int id)
        {
            var funcionario = _context.Funcionarios
                .Include(f => f.Cinema)
                .FirstOrDefault(f => f.Id == id);
            if (funcionario == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Funcionario com ID {id} nao encontrado.");
            }
            return funcionario;
        }

        public List<Funcionario> ListarFuncionarios()
        {
            return _context.Funcionarios
                .Include(f => f.Cinema)
                .ToList();
        }

        public List<Funcionario> ListarPorCargo(CargoFuncionario cargo)
        {
            return _context.Funcionarios
                .Include(f => f.Cinema)
                .Where(f => f.Cargo == cargo)
                .ToList();
        }

        public List<Funcionario> ListarPorCinema(int cinemaId)
        {
            return _context.Funcionarios
                .Include(f => f.Cinema)
                .Where(f => f.Cinema != null && f.Cinema.Id == cinemaId)
                .ToList();
        }

        public void AtualizarFuncionario(int id, string? nome = null, CargoFuncionario? cargo = null, Cinema? cinema = null)
        {
            var funcionario = ObterFuncionario(id);

            if (!string.IsNullOrWhiteSpace(nome))
            {
                funcionario.Nome = nome;
            }

            if (cargo.HasValue)
            {
                funcionario.Cargo = cargo.Value;
            }

            if (cinema != null && funcionario.Cinema?.Id != cinema.Id)
            {
                funcionario.Cinema = cinema;
            }

            funcionario.DataAtualizacao = DateTime.Now;

            _context.SaveChanges();
        }

        public void DeletarFuncionario(int id)
        {
            var funcionario = ObterFuncionario(id);
            _context.Funcionarios.Remove(funcionario);
            _context.SaveChanges();
        }
    }
}
