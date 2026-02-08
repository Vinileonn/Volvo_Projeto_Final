using cineflow.controladores;
using cineflow.modelos;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuEscalasLimpeza
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuEscalasLimpeza(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Escalas de Limpeza");
                MenuHelper.MostrarOpcoes(
                    "Criar Escala",
                    "Listar Escalas",
                    "Listar por Sala",
                    "Listar por Funcionario",
                    "Deletar Escala");

                var opcao = MenuHelper.LerOpcaoInteira(0, 5);

                switch (opcao)
                {
                    case 1:
                        CriarEscala();
                        break;
                    case 2:
                        ListarEscalas();
                        break;
                    case 3:
                        ListarEscalasPorSala();
                        break;
                    case 4:
                        ListarEscalasPorFuncionario();
                        break;
                    case 5:
                        DeletarEscala();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // CRIAR - 
        private void CriarEscala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Criar Escala de Limpeza");

            try
            {
                var (salas, mensagemSalas) = administradorControlador.SalaControlador.ListarSalas();
                if (salas.Count == 0)
                {
                    MenuHelper.ExibirMensagem("Nenhuma sala disponivel.");
                    MenuHelper.Pausar();
                    return;
                }

                Console.WriteLine("\nSalas Disponíveis:");
                ExibirSalasTabela(salas);

                var salaId = MenuHelper.LerInteiro("ID da Sala: ", 1, salas.Max(s => s.Id));
                var sala = salas.FirstOrDefault(s => s.Id == salaId);
                if (sala == null)
                {
                    MenuHelper.ExibirMensagem("Sala não encontrada.");
                    MenuHelper.Pausar();
                    return;
                }

                var (funcionarios, mensagemFuncionarios) = administradorControlador.FuncionarioControlador.ListarFuncionarios();
                if (funcionarios.Count == 0)
                {
                    MenuHelper.ExibirMensagem("Nenhum funcionario disponivel.");
                    MenuHelper.Pausar();
                    return;
                }

                Console.WriteLine("\nFuncionarios Disponíveis:");
                ExibirFuncionariosTabela(funcionarios);

                var funcionarioId = MenuHelper.LerInteiro("ID do Funcionario: ", 1, funcionarios.Max(f => f.Id));
                var funcionario = funcionarios.FirstOrDefault(f => f.Id == funcionarioId);
                if (funcionario == null)
                {
                    MenuHelper.ExibirMensagem("Funcionario não encontrado.");
                    MenuHelper.Pausar();
                    return;
                }

                Console.Write("Data/Hora de Inicio (formato dd/MM/yyyy HH:mm): ");
                var inicioStr = Console.ReadLine();
                if (!DateTime.TryParseExact(inicioStr, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out var inicio))
                {
                    MenuHelper.ExibirMensagem("Formato inválido.");
                    MenuHelper.Pausar();
                    return;
                }

                Console.Write("Data/Hora de Fim (formato dd/MM/yyyy HH:mm): ");
                var fimStr = Console.ReadLine();
                if (!DateTime.TryParseExact(fimStr, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out var fim))
                {
                    MenuHelper.ExibirMensagem("Formato inválido.");
                    MenuHelper.Pausar();
                    return;
                }

                var escala = new EscalaLimpeza(0, sala, funcionario, inicio, fim);
                var (sucesso, mensagem) = administradorControlador.LimpezaControlador.CriarEscala(sala, funcionario, inicio, fim);

                MenuHelper.ExibirMensagem(mensagem);
            }
            catch (Exception ex)
            {
                MenuHelper.ExibirMensagem($"Erro: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ListarEscalas()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Escalas de Limpeza");

            var (escalas, mensagem) = administradorControlador.LimpezaControlador.ListarEscalas();
            MenuHelper.ExibirMensagem(mensagem);

            if (escalas.Count > 0)
            {
                ExibirEscalasTabela(escalas);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void ListarEscalasPorSala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Escalas por Sala");

            var (salas, mensagem) = administradorControlador.SalaControlador.ListarSalas();
            if (salas.Count == 0)
            {
                MenuHelper.ExibirMensagem("Nenhuma sala disponivel.");
                MenuHelper.Pausar();
                return;
            }

            ExibirSalasTabela(salas);

            var salaId = MenuHelper.LerInteiro("ID da Sala: ", 1, salas.Max(s => s.Id));
            var (escalas, mensagemEscalas) = administradorControlador.LimpezaControlador.ListarPorSala(salaId);

            MenuHelper.ExibirMensagem(mensagemEscalas);

            if (escalas.Count > 0)
            {
                ExibirEscalasTabela(escalas);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void ListarEscalasPorFuncionario()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Escalas por Funcionario");

            var (funcionarios, mensagem) = administradorControlador.FuncionarioControlador.ListarFuncionarios();
            if (funcionarios.Count == 0)
            {
                MenuHelper.ExibirMensagem("Nenhum funcionario disponivel.");
                MenuHelper.Pausar();
                return;
            }

            ExibirFuncionariosTabela(funcionarios);

            var funcionarioId = MenuHelper.LerInteiro("ID do Funcionario: ", 1, funcionarios.Max(f => f.Id));
            var (escalas, mensagemEscalas) = administradorControlador.LimpezaControlador.ListarPorFuncionario(funcionarioId);

            MenuHelper.ExibirMensagem(mensagemEscalas);

            if (escalas.Count > 0)
            {
                ExibirEscalasTabela(escalas);
            }

            MenuHelper.Pausar();
        // DELETAR - 
        }

        private void DeletarEscala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Deletar Escala de Limpeza");

            var id = MenuHelper.LerInteiro("ID da Escala: ", 1, 999999);

            Console.Write("Tem certeza que deseja deletar? (sim/nao): ");
            var confirmacao = Console.ReadLine();

            if (confirmacao?.ToLower() == "sim")
            {
                var (sucesso, mensagem) = administradorControlador.LimpezaControlador.DeletarEscala(id);
                MenuHelper.ExibirMensagem(mensagem);
            }
            else
            {
                MenuHelper.ExibirMensagem("Operacao cancelada.");
            }

            MenuHelper.Pausar();
        }

        private void ExibirSalasTabela(List<Sala> salas)
        {
            Console.WriteLine("\n{0,-4} {1,-25} {2,-12} {3,-20} {4,-8}", "ID", "Nome", "Capacidade", "Cinema", "Tipo");
            Console.WriteLine(new string('-', 80));
            foreach (var sala in salas)
            {
                Console.WriteLine("{0,-4} {1,-25} {2,-12} {3,-20} {4,-8}", sala.Id, sala.Nome, sala.Capacidade, (sala.Cinema != null ? sala.Cinema.Nome : "Não informado"), sala.Tipo);
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

        private void ExibirEscalasTabela(List<EscalaLimpeza> escalas)
        {
            Console.WriteLine("\n{0,-4} {1,-25} {2,-25} {3,-20} {4,-20}", "ID", "Sala", "Funcionario", "Inicio", "Fim");
            Console.WriteLine(new string('-', 100));
            foreach (var escala in escalas)
            {
                Console.WriteLine("{0,-4} {1,-25} {2,-25} {3,-20} {4,-20}", escala.Id, (escala.Sala != null ? escala.Sala.Nome : "N/A"), (escala.Funcionario != null ? escala.Funcionario.Nome : "N/A"), escala.Inicio.ToString("dd/MM/yyyy HH:mm"), escala.Fim.ToString("dd/MM/yyyy HH:mm"));
            }
        }
    }
}
