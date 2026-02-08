using cineflow.controladores;
using cineflow.modelos;
using cineflow.enumeracoes;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuSalas
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuSalas(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Salas");
                MenuHelper.MostrarOpcoes(
                    "Criar Sala",
                    "Listar Salas",
                    "Obter Detalhes de Sala",
                    "Gerar Assentos para Sala",
                    "Atualizar Sala",
                    "Deletar Sala");

                var opcao = MenuHelper.LerOpcaoInteira(0, 6);

                switch (opcao)
                {
                    case 1:
                        CriarSala();
                        break;
                    case 2:
                        ListarSalas();
                        break;
                    case 3:
                        ObterDetalhesSala();
                        break;
                    case 4:
                        GerarAssentosParaSala();
                        break;
                    case 5:
                        AtualizarSala();
                        break;
                    case 6:
                        DeletarSala();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // CRIAR - 
        private void CriarSala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Criar Sala");

            try
            {
                var (cinemas, mensagem) = administradorControlador.CinemaControlador.ListarCinemas();
                if (cinemas.Count == 0)
                {
                    MenuHelper.ExibirMensagem("Nenhum cinema disponivel. Crie um cinema primeiro.");
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

                var nome = MenuHelper.LerTextoNaoVazio("Nome da Sala: ");
                var capacidade = MenuHelper.LerInteiro("Capacidade (numero de assentos): ", 1, 500);

                Console.WriteLine("Tipo de Sala:");
                MenuHelper.MostrarOpcoes("Normal", "XD", "VIP", "4D");
                var tipoOpcao = MenuHelper.LerOpcaoInteira(1, 4);
                var tipo = tipoOpcao == 1 ? TipoSala.Normal :
                           tipoOpcao == 2 ? TipoSala.XD :
                           tipoOpcao == 3 ? TipoSala.VIP : TipoSala.QuatroD;

                var sala = new Sala(0, nome, capacidade, cinema, tipo);
                var (sucesso, mensagemResultado) = administradorControlador.SalaControlador.CriarSala(sala);

                MenuHelper.ExibirMensagem(mensagemResultado);
            }
            catch (Exception ex)
            {
                MenuHelper.ExibirMensagem($"Erro: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ListarSalas()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Salas");

            var (salas, mensagem) = administradorControlador.SalaControlador.ListarSalas();
            MenuHelper.ExibirMensagem(mensagem);

            if (salas.Count > 0)
            {
                ExibirSalasTabela(salas);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void ObterDetalhesSala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Obter Detalhes de Sala");

            var id = MenuHelper.LerInteiro("ID da Sala: ", 1, 999999);
            var (sala, mensagem) = administradorControlador.SalaControlador.ObterSala(id);

            MenuHelper.ExibirMensagem(mensagem);

            if (sala != null)
            {
                Console.WriteLine($"\nID: {sala.Id}");
                Console.WriteLine($"Nome: {sala.Nome}");
                Console.WriteLine($"Capacidade: {sala.Capacidade}");
                var cinemaName = sala.Cinema != null ? sala.Cinema.Nome : "Não informado";
                Console.WriteLine($"Cinema: {cinemaName}");
                Console.WriteLine($"Tipo: {sala.Tipo}");
                Console.WriteLine($"Assentos: {sala.Assentos.Count}");
            }

            MenuHelper.Pausar();
        }
// CRIAR - 
        
        private void GerarAssentosParaSala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Gerar Assentos para Sala");

            var (salas, mensagem) = administradorControlador.SalaControlador.ListarSalas();
            if (salas.Count == 0)
            {
                MenuHelper.ExibirMensagem("Nenhuma sala disponivel.");
                MenuHelper.Pausar();
                return;
            }

            ExibirSalasTabela(salas);

            var salaId = MenuHelper.LerInteiro("ID da Sala: ", 1, salas.Max(s => s.Id));
            var (sucesso, mensagemResultado) = administradorControlador.SalaControlador.GerarAssentosParaSala(salaId);

            MenuHelper.ExibirMensagem(mensagemResultado);
            MenuHelper.Pausar();
        // ATUALIZAR - 
        }

        private void AtualizarSala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Atualizar Sala");

            var id = MenuHelper.LerInteiro("ID da Sala: ", 1, 999999);

            var nome = MenuHelper.LerTextoOpcional("Novo Nome (ou deixe em branco): ");

            var capacidade = MenuHelper.LerTextoOpcional("Nova Capacidade (ou deixe em branco): ");
            int? capacidadeInt = string.IsNullOrWhiteSpace(capacidade) ? null : int.Parse(capacidade);

            var (sucesso, mensagem) = administradorControlador.SalaControlador.AtualizarSala(id, nome, capacidadeInt);

            MenuHelper.ExibirMensagem(mensagem);
            MenuHelper.Pausar();
        // DELETAR - 
        }

        private void DeletarSala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Deletar Sala");

            var id = MenuHelper.LerInteiro("ID da Sala: ", 1, 999999);

            Console.Write("Tem certeza que deseja deletar? (sim/nao): ");
            var confirmacao = Console.ReadLine();

            if (confirmacao?.ToLower() == "sim")
            {
                var (sucesso, mensagem) = administradorControlador.SalaControlador.DeletarSala(id);
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
                var cinemaName = sala.Cinema != null ? sala.Cinema.Nome : "Não informado";
                Console.WriteLine("{0,-4} {1,-25} {2,-12} {3,-20} {4,-8}", sala.Id, sala.Nome, sala.Capacidade, cinemaName, sala.Tipo);
            }
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
