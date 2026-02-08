using cineflow.controladores;
using cineflow.modelos;
using cineflow.enumeracoes;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuFuncionarios
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuFuncionarios(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Funcionarios");
                MenuHelper.MostrarOpcoes(
                    "Criar Funcionario",
                    "Listar Funcionarios",
                    "Listar por Cargo",
                    "Listar por Cinema",
                    "Obter Detalhes de Funcionario",
                    "Atualizar Funcionario",
                    "Deletar Funcionario");

                var opcao = MenuHelper.LerOpcaoInteira(0, 7);

                switch (opcao)
                {
                    case 1:
                        CriarFuncionario();
                        break;
                    case 2:
                        ListarFuncionarios();
                        break;
                    case 3:
                        ListarFuncionariosPorCargo();
                        break;
                    case 4:
                        ListarFuncionariosPorCinema();
                        break;
                    case 5:
                        ObterDetalhesFuncionario();
                        break;
                    case 6:
                        AtualizarFuncionario();
                        break;
                    case 7:
                        DeletarFuncionario();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // CRIAR - 
        private void CriarFuncionario()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Criar Funcionario");

            try
            {
                var nome = MenuHelper.LerTextoNaoVazio("Nome do Funcionario: ");

                Console.WriteLine("Cargo do Funcionario:");
                MenuHelper.MostrarOpcoes("Atendente de Ingressos", "Atendente de Alimentos", "Limpeza");
                var cargoOpcao = MenuHelper.LerOpcaoInteira(1, 3);
                var cargo = cargoOpcao == 1 ? CargoFuncionario.AtendenteIngressos :
                            cargoOpcao == 2 ? CargoFuncionario.AtendenteAlimentos : CargoFuncionario.Limpeza;

                var (cinemas, mensagem) = administradorControlador.CinemaControlador.ListarCinemas();
                if (cinemas.Count == 0)
                {
                    MenuHelper.ExibirMensagem("Nenhum cinema disponivel.");
                    MenuHelper.Pausar();
                    return;
                }

                Console.WriteLine("\nCinemas Disponíveis:");
                ExibirCinemasTabela(cinemas);

                var cinemaId = MenuHelper.LerInteiro("ID do Cinema: ", 1, cinemas.Max(c => c.Id));
                var cinema = cinemas.FirstOrDefault(c => c.Id == cinemaId);

                if (cinema == null)
                {
                    MenuHelper.ExibirMensagem("Cinema não encontrado.");
                    MenuHelper.Pausar();
                    return;
                }

                var funcionario = new Funcionario(0, nome, cargo, cinema);
                var (sucesso, mensagemResultado) = administradorControlador.FuncionarioControlador.CriarFuncionario(funcionario);

                MenuHelper.ExibirMensagem(mensagemResultado);
            }
            catch (Exception ex)
            {
                MenuHelper.ExibirMensagem($"Erro: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ListarFuncionarios()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Funcionarios");

            var (funcionarios, mensagem) = administradorControlador.FuncionarioControlador.ListarFuncionarios();
            MenuHelper.ExibirMensagem(mensagem);

            if (funcionarios.Count > 0)
            {
                ExibirFuncionariosTabela(funcionarios);
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ListarFuncionariosPorCargo()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Funcionarios por Cargo");

            Console.WriteLine("Selecione o Cargo:");
                MenuHelper.MostrarOpcoes("Atendente de Ingressos", "Atendente de Alimentos", "Limpeza");
                var cargoOpcao = MenuHelper.LerOpcaoInteira(1, 3);
                var cargo = cargoOpcao == 1 ? CargoFuncionario.AtendenteIngressos :
                            cargoOpcao == 2 ? CargoFuncionario.AtendenteAlimentos : CargoFuncionario.Limpeza;
            var (funcionarios, mensagem) = administradorControlador.FuncionarioControlador.ListarPorCargo(cargo);
            MenuHelper.ExibirMensagem(mensagem);

            if (funcionarios.Count > 0)
            {
                ExibirFuncionariosTabela(funcionarios);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void ListarFuncionariosPorCinema()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Funcionarios por Cinema");

            var (cinemas, mensagem) = administradorControlador.CinemaControlador.ListarCinemas();
            if (cinemas.Count == 0)
            {
                MenuHelper.ExibirMensagem("Nenhum cinema disponivel.");
                MenuHelper.Pausar();
                return;
            }

            ExibirCinemasTabela(cinemas);

            var cinemaId = MenuHelper.LerInteiro("ID do Cinema: ", 1, cinemas.Max(c => c.Id));
            var (funcionarios, mensagemFuncionarios) = administradorControlador.FuncionarioControlador.ListarPorCinema(cinemaId);

            MenuHelper.ExibirMensagem(mensagemFuncionarios);

            if (funcionarios.Count > 0)
            {
                ExibirFuncionariosTabela(funcionarios);
            }

            MenuHelper.Pausar();
        // LER - 
        }

        private void ObterDetalhesFuncionario()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Obter Detalhes de Funcionario");

            var id = MenuHelper.LerInteiro("ID do Funcionario: ", 1, 999999);
            var (funcionario, mensagem) = administradorControlador.FuncionarioControlador.ObterFuncionario(id);

            MenuHelper.ExibirMensagem(mensagem);

            if (funcionario != null)
            {
                Console.WriteLine($"\nID: {funcionario.Id}");
                Console.WriteLine($"Nome: {funcionario.Nome}");
                Console.WriteLine($"Cargo: {funcionario.Cargo}");
                Console.WriteLine($"Cinema: {(funcionario.Cinema != null ? funcionario.Cinema.Nome : "Não informado")}");
            }

            MenuHelper.Pausar();
        // ATUALIZAR - 
        }

        private void AtualizarFuncionario()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Atualizar Funcionario");

            var id = MenuHelper.LerInteiro("ID do Funcionario: ", 1, 999999);

            var nome = MenuHelper.LerTextoOpcional("Novo Nome (ou deixe em branco): ");

            var (sucesso, mensagem) = administradorControlador.FuncionarioControlador.AtualizarFuncionario(id, nome);

            MenuHelper.ExibirMensagem(mensagem);
        // DELETAR - 
            MenuHelper.Pausar();
        }

        private void DeletarFuncionario()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Deletar Funcionario");

            var id = MenuHelper.LerInteiro("ID do Funcionario: ", 1, 999999);

            Console.Write("Tem certeza que deseja deletar? (sim/nao): ");
            var confirmacao = Console.ReadLine();

            if (confirmacao?.ToLower() == "sim")
            {
                var (sucesso, mensagem) = administradorControlador.FuncionarioControlador.DeletarFuncionario(id);
                MenuHelper.ExibirMensagem(mensagem);
            }
            else
            {
                MenuHelper.ExibirMensagem("Operacao cancelada.");
            }

            MenuHelper.Pausar();
        }

        private void ExibirCinemasTabela(List<Cinema> cinemas)
        {
            Console.WriteLine("\n{0,-4} {1,-30} {2,-35}", "ID", "Nome", "Endereco");
            Console.WriteLine(new string('-', 75));
            foreach (var cinema in cinemas)
            {
                Console.WriteLine("{0,-4} {1,-30} {2,-35}", cinema.Id, cinema.Nome, cinema.Endereco);
            }
        }

        private void ExibirFuncionariosTabela(List<Funcionario> funcionarios)
        {
            Console.WriteLine("\n{0,-4} {1,-25} {2,-18} {3,-30}", "ID", "Nome", "Cargo", "Cinema");
            Console.WriteLine(new string('-', 80));
            foreach (var func in funcionarios)
            {
                Console.WriteLine("{0,-4} {1,-25} {2,-18} {3,-30}", func.Id, func.Nome, func.Cargo, (func.Cinema != null ? func.Cinema.Nome : "Não informado"));
            }
        }
    }
}
