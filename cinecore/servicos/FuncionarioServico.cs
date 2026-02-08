using cinecore.modelos;
using cinecore.enums;
using cinecore.excecoes;

namespace cinecore.servicos
{
    public class FuncionarioServico
    {
        private readonly List<Funcionario> funcionarios;

        public FuncionarioServico()
        {
            funcionarios = new List<Funcionario>();
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

            funcionario.Id = funcionarios.Count > 0 ? funcionarios.Max(f => f.Id) + 1 : 1;
            funcionario.DataCriacao = DateTime.Now;
            funcionarios.Add(funcionario);

            // Mantem uma lista por cinema para consultas rapidas.
            if (!funcionario.Cinema.Funcionarios.Contains(funcionario))
            {
                funcionario.Cinema.Funcionarios.Add(funcionario);
            }
        }

        public Funcionario ObterFuncionario(int id)
        {
            var funcionario = funcionarios.FirstOrDefault(f => f.Id == id);
            if (funcionario == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Funcionario com ID {id} nao encontrado.");
            }
            return funcionario;
        }

        public List<Funcionario> ListarFuncionarios()
        {
            return new List<Funcionario>(funcionarios);
        }

        public List<Funcionario> ListarPorCargo(CargoFuncionario cargo)
        {
            return funcionarios.Where(f => f.Cargo == cargo).ToList();
        }

        public List<Funcionario> ListarPorCinema(int cinemaId)
        {
            return funcionarios.Where(f => f.Cinema != null && f.Cinema.Id == cinemaId).ToList();
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
                // Remove da lista antiga e adiciona na nova para manter coerencia.
                if (funcionario.Cinema != null)
                {
                    funcionario.Cinema.Funcionarios.Remove(funcionario);
                }
                funcionario.Cinema = cinema;
                if (!cinema.Funcionarios.Contains(funcionario))
                {
                    cinema.Funcionarios.Add(funcionario);
                }
            }

            funcionario.DataAtualizacao = DateTime.Now;
        }

        public void DeletarFuncionario(int id)
        {
            var funcionario = ObterFuncionario(id);
            funcionarios.Remove(funcionario);

            if (funcionario.Cinema != null)
            {
                funcionario.Cinema.Funcionarios.Remove(funcionario);
            }
        }
    }
}
