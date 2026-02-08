using cineflow.controladores;
using cineflow.enumeracoes;
using cineflow.modelos;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuAlugueis
    {
        private readonly AluguelSalaControlador aluguelControlador;
        private readonly SalaControlador salaControlador;

        public MenuAlugueis(AluguelSalaControlador aluguelControlador, SalaControlador salaControlador)
        {
            this.aluguelControlador = aluguelControlador;
            this.salaControlador = salaControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Alugueis de Sala");
                MenuHelper.MostrarOpcoes(
                    "Criar aluguel",
                    "Listar alugueis",
                    "Listar por status",
                    "Aprovar aluguel",
                    "Cancelar aluguel");

                var opcao = MenuHelper.LerOpcaoInteira(0, 5);

                switch (opcao)
                {
                    case 1:
                        CriarAluguel();
                        break;
                    case 2:
                        ListarAlugueis();
                        break;
                    case 3:
                        ListarPorStatus();
                        break;
                    case 4:
                        AprovarAluguel();
                        break;
                    case 5:
                        CancelarAluguel();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CriarAluguel()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Criar Aluguel");

            var (salas, mensagem) = salaControlador.ListarSalas();
            MenuHelper.ExibirMensagem(mensagem);
            if (salas.Count == 0)
            {
                MenuHelper.Pausar();
                return;
            }

            ExibirSalasTabela(salas);
            var salaId = MenuHelper.LerInteiro("ID da Sala: ", 1, salas.Max(s => s.Id));
            var sala = salas.FirstOrDefault(s => s.Id == salaId);
            if (sala == null)
            {
                MenuHelper.ExibirMensagem("Sala nao encontrada.");
                MenuHelper.Pausar();
                return;
            }

            var inicio = MenuHelper.LerDataHora("Data/Hora de inicio (dd/MM/yyyy HH:mm): ");
            var fim = MenuHelper.LerDataHora("Data/Hora de fim (dd/MM/yyyy HH:mm): ");
            var nomeCliente = MenuHelper.LerTextoNaoVazio("Nome do cliente: ");
            var contato = MenuHelper.LerTextoNaoVazio("Contato: ");
            var motivo = MenuHelper.LerTextoNaoVazio("Motivo: ");
            var valor = MenuHelper.LerDecimal("Valor do aluguel: ", 0m, 100000m);
            bool pacoteAniversario = MenuHelper.Confirmar("Pacote aniversario?");

            var aluguel = new AluguelSala(0, sala, inicio, fim, nomeCliente, contato, motivo, valor, StatusAluguel.Solicitado, null, pacoteAniversario);
            var (sucesso, msg) = aluguelControlador.SolicitarAluguel(aluguel);
            MenuHelper.ExibirMensagem(msg);
            MenuHelper.Pausar();
        }

        private void ListarAlugueis()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Alugueis");

            var (alugueis, mensagem) = aluguelControlador.ListarAlugueis();
            MenuHelper.ExibirMensagem(mensagem);
            if (alugueis.Count > 0)
            {
                ExibirAlugueisTabela(alugueis);
            }

            MenuHelper.Pausar();
        }

        private void ListarPorStatus()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar por Status");

            Console.WriteLine("Status:");
            MenuHelper.MostrarOpcoes("Solicitado", "Aprovado", "Cancelado");
            var opcao = MenuHelper.LerOpcaoInteira(1, 3);
            var status = (StatusAluguel)opcao;

            var (alugueis, mensagem) = aluguelControlador.ListarPorStatus(status);
            MenuHelper.ExibirMensagem(mensagem);
            if (alugueis.Count > 0)
            {
                ExibirAlugueisTabela(alugueis);
            }

            MenuHelper.Pausar();
        }

        private void AprovarAluguel()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Aprovar Aluguel");

            var id = MenuHelper.LerInteiro("ID do aluguel: ", 1, 999999);
            var valor = MenuHelper.LerDecimal("Valor do aluguel: ", 0m, 100000m);

            var (sucesso, msg) = aluguelControlador.AprovarAluguel(id, valor);
            MenuHelper.ExibirMensagem(msg);
            MenuHelper.Pausar();
        }

        private void CancelarAluguel()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Cancelar Aluguel");

            var id = MenuHelper.LerInteiro("ID do aluguel: ", 1, 999999);
            var (sucesso, msg) = aluguelControlador.CancelarAluguel(id);
            MenuHelper.ExibirMensagem(msg);
            MenuHelper.Pausar();
        }

        private static void ExibirSalasTabela(List<Sala> salas)
        {
            Console.WriteLine("\n{0,-4} {1,-25} {2,-12} {3,-20} {4,-8}", "ID", "Nome", "Capacidade", "Cinema", "Tipo");
            Console.WriteLine(new string('-', 80));
            foreach (var sala in salas)
            {
                var cinemaName = sala.Cinema != null ? sala.Cinema.Nome : "Nao informado";
                Console.WriteLine("{0,-4} {1,-25} {2,-12} {3,-20} {4,-8}", sala.Id, sala.Nome, sala.Capacidade, cinemaName, sala.Tipo);
            }
        }

        private static void ExibirAlugueisTabela(List<AluguelSala> alugueis)
        {
            Console.WriteLine("\n{0,-4} {1,-18} {2,-16} {3,-16} {4,-8} {5,-8} {6,-8}", "ID", "Cliente", "Inicio", "Fim", "Sala", "Status", "Pacote");
            Console.WriteLine(new string('-', 95));
            foreach (var aluguel in alugueis)
            {
                Console.WriteLine("{0,-4} {1,-18} {2,-16} {3,-16} {4,-8} {5,-8} {6,-8}",
                    aluguel.Id,
                    Truncar(aluguel.NomeCliente, 16),
                    aluguel.Inicio.ToString("dd/MM/yyyy HH:mm"),
                    aluguel.Fim.ToString("dd/MM/yyyy HH:mm"),
                    Truncar(aluguel.Sala.Nome, 6),
                    aluguel.Status,
                    aluguel.PacoteAniversario ? "Sim" : "Nao");
            }
        }

        private static string Truncar(string texto, int max)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return string.Empty;
            }

            if (texto.Length <= max)
            {
                return texto;
            }

            return texto.Substring(0, max - 3) + "...";
        }
    }
}
