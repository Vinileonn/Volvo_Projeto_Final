using cinema.modelos;
using cinema.enumeracoes;
using cinema.excecoes;

namespace cinema.servicos
{
    // Renomeado de FuncionarioService para FuncionarioServico.
    public class FuncionarioServico
    {
        private readonly List<Funcionario> funcionarios;

        public FuncionarioServico()
        {
            funcionarios = new List<Funcionario>();
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
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
            funcionarios.Add(funcionario);

            // Mantem uma lista por cinema para consultas rapidas.
            if (!funcionario.Cinema.Funcionarios.Contains(funcionario))
            {
                funcionario.Cinema.Funcionarios.Add(funcionario);
            }
        }
// ANTIGO: // LER - 
// LER - 
        public Funcionario ObterFuncionario(int id)
        {
            var funcionario = funcionarios.FirstOrDefault(f => f.Id == id);
            if (funcionario == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Funcionario com ID {id} nao encontrado.");
            }
            return funcionario;
        }
// ANTIGO: // LER - 
// LER - 
        public List<Funcionario> ListarFuncionarios()
        {
            return new List<Funcionario>(funcionarios);
        }
// ANTIGO: // LER - 
// LER - 
        public List<Funcionario> ListarPorCargo(CargoFuncionario cargo)
        {
            return funcionarios.Where(f => f.Cargo == cargo).ToList();
        }
// ANTIGO: // LER - 
// LER - 
        public List<Funcionario> ListarPorCinema(int cinemaId)
        {
            return funcionarios.Where(f => f.Cinema != null && f.Cinema.Id == cinemaId).ToList();
        }
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
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
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
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





