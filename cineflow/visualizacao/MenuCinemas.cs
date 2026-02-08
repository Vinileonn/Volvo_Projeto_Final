using cineflow.controladores;
using cineflow.modelos;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuCinemas
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuCinemas(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Cinemas");
                MenuHelper.MostrarOpcoes(
                    "Criar Cinema",
                    "Listar Cinemas",
                    "Obter Detalhes de Cinema",
                    "Atualizar Cinema",
                    "Deletar Cinema");

                var opcao = MenuHelper.LerOpcaoInteira(0, 5);

                switch (opcao)
                {
                    case 1:
                        CriarCinema();
                        break;
                    case 2:
                        ListarCinemas();
                        break;
                    case 3:
                        ObterDetalhesCinema();
                        break;
                    case 4:
                        AtualizarCinema();
                        break;
                    case 5:
                        DeletarCinema();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // CRIAR - 
        private void CriarCinema()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Criar Cinema");

            try
            {
                var nome = MenuHelper.LerTextoNaoVazio("Nome do Cinema: ");
                var endereco = MenuHelper.LerTextoNaoVazio("Endereco: ");

                var cinema = new Cinema(0, nome, endereco);
                var (sucesso, mensagem) = administradorControlador.CinemaControlador.CriarCinema(cinema);

                MenuHelper.ExibirMensagem(mensagem);
            }
            catch (Exception ex)
            {
                MenuHelper.ExibirMensagem($"Erro: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ListarCinemas()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Cinemas");

            var (cinemas, mensagem) = administradorControlador.CinemaControlador.ListarCinemas();
            MenuHelper.ExibirMensagem(mensagem);

            if (cinemas.Count > 0)
            {
                ExibirCinemasTabela(cinemas);
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ObterDetalhesCinema()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Obter Detalhes de Cinema");

            var id = MenuHelper.LerInteiro("ID do Cinema: ", 1, 999999);
            var (cinema, mensagem) = administradorControlador.CinemaControlador.ObterCinema(id);

            MenuHelper.ExibirMensagem(mensagem);

            if (cinema != null)
            {
                Console.WriteLine($"\nID: {cinema.Id}");
                Console.WriteLine($"Nome: {cinema.Nome}");
                Console.WriteLine($"Endereco: {cinema.Endereco}");
                Console.WriteLine($"Salas: {cinema.Salas.Count}");
            }

            MenuHelper.Pausar();
        }
// ATUALIZAR - 
        
        private void AtualizarCinema()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Atualizar Cinema");

            var id = MenuHelper.LerInteiro("ID do Cinema: ", 1, 999999);

            var nome = MenuHelper.LerTextoOpcional("Novo Nome (ou deixe em branco): ");

            var endereco = MenuHelper.LerTextoOpcional("Novo Endereco (ou deixe em branco): ");

            var (sucesso, mensagem) = administradorControlador.CinemaControlador.AtualizarCinema(id, nome, endereco);

            MenuHelper.ExibirMensagem(mensagem);
            MenuHelper.Pausar();
        // DELETAR - 
        }

        private void DeletarCinema()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Deletar Cinema");

            var id = MenuHelper.LerInteiro("ID do Cinema: ", 1, 999999);

            Console.Write("Tem certeza que deseja deletar? (sim/nao): ");
            var confirmacao = Console.ReadLine();

            if (confirmacao?.ToLower() == "sim")
            {
                var (sucesso, mensagem) = administradorControlador.CinemaControlador.DeletarCinema(id);
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
    }
}
