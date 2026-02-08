using cineflow.controladores;
using cineflow.modelos;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuFilmes
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuFilmes(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Filmes");
                MenuHelper.MostrarOpcoes(
                    "Criar Filme",
                    "Listar Filmes",
                    "Buscar Filme por Titulo",
                    "Obter Detalhes de Filme",
                    "Atualizar Filme",
                    "Deletar Filme");

                var opcao = MenuHelper.LerOpcaoInteira(0, 6);

                switch (opcao)
                {
                    case 1:
                        CriarFilme();
                        break;
                    case 2:
                        ListarFilmes();
                        break;
                    case 3:
                        BuscarFilmePorTitulo();
                        break;
                    case 4:
                        ObterDetalhesFilme();
                        break;
                    case 5:
                        AtualizarFilme();
                        break;
                    case 6:
                        DeletarFilme();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // CRIAR - 
        private void CriarFilme()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Criar Filme");

            try
            {
                var titulo = MenuHelper.LerTextoNaoVazio("Titulo: ");
                var duracao = MenuHelper.LerInteiro("Duracao (minutos): ", 1, 600);
                var genero = MenuHelper.LerTextoNaoVazio("Genero: ");
                var ano = MenuHelper.LerInteiro("Ano de Lancamento: ", 1900, 2100);
                var anoLancamento = new DateTime(ano, 1, 1);
                var eh3D = MenuHelper.Confirmar("É um filme 3D?");

                var filme = new Filme(0, titulo, duracao, genero, anoLancamento, eh3D);
                var (sucesso, mensagem) = administradorControlador.FilmeControlador.CriarFilme(filme);

                MenuHelper.ExibirMensagem(mensagem);
            }
            catch (Exception ex)
            {
                MenuHelper.ExibirMensagem($"Erro: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ListarFilmes()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Filmes");

            var (filmes, mensagem) = administradorControlador.FilmeControlador.ListarFilmes();
            MenuHelper.ExibirMensagem(mensagem);

            if (filmes.Count > 0)
            {
                ExibirFilmesTabela(filmes);
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void BuscarFilmePorTitulo()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Buscar Filme por Titulo");

            var titulo = MenuHelper.LerTextoNaoVazio("Titulo: ");
            var (filmes, mensagem) = administradorControlador.FilmeControlador.BuscarPorTitulo(titulo);

            MenuHelper.ExibirMensagem(mensagem);

            if (filmes.Count > 0)
            {
                ExibirFilmesTabela(filmes);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void ObterDetalhesFilme()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Obter Detalhes de Filme");

            var id = MenuHelper.LerInteiro("ID do Filme: ", 1, 999999);
            var (filme, mensagem) = administradorControlador.FilmeControlador.ObterFilme(id);

            MenuHelper.ExibirMensagem(mensagem);

            if (filme != null)
            {
                Console.WriteLine($"\nID: {filme.Id}");
                Console.WriteLine($"Título: {filme.Titulo}");
                Console.WriteLine($"Duração: {filme.Duracao} minutos");
                Console.WriteLine($"Gênero: {filme.Genero}");
                Console.WriteLine($"Ano de Lançamento: {filme.AnoLancamento.Year}");
                Console.WriteLine($"Filme 3D: {(filme.Eh3D ? "Sim" : "Não")}");
                Console.WriteLine($"Sessões: {filme.Sessoes.Count}");
            }

            MenuHelper.Pausar();
        // ATUALIZAR - 
        }

        private void AtualizarFilme()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Atualizar Filme");

            var id = MenuHelper.LerInteiro("ID do Filme: ", 1, 999999);

            var titulo = MenuHelper.LerTextoOpcional("Novo Titulo (ou deixe em branco para manter): ");

            var duracao = MenuHelper.LerTextoOpcional("Nova Duracao em minutos (ou deixe em branco): ");
            int? duracaoInt = string.IsNullOrWhiteSpace(duracao) ? null : int.Parse(duracao);

            var genero = MenuHelper.LerTextoOpcional("Novo Genero (ou deixe em branco): ");

            var anoStr = MenuHelper.LerTextoOpcional("Novo Ano de Lancamento (ou deixe em branco): ");
            DateTime? anoLancamento = null;
            if (!string.IsNullOrWhiteSpace(anoStr) && int.TryParse(anoStr, out int ano))
            {
                anoLancamento = new DateTime(ano, 1, 1);
            }

            var (sucesso, mensagem) = administradorControlador.FilmeControlador.AtualizarFilme(
                id, titulo, duracaoInt, genero, anoLancamento);

            MenuHelper.ExibirMensagem(mensagem);
            MenuHelper.Pausar();
        // DELETAR - 
        }

        private void DeletarFilme()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Deletar Filme");

            var id = MenuHelper.LerInteiro("ID do Filme: ", 1, 999999);

            Console.Write("Tem certeza que deseja deletar? (sim/nao): ");
            var confirmacao = Console.ReadLine();

            if (confirmacao?.ToLower() == "sim")
            {
                var (sucesso, mensagem) = administradorControlador.FilmeControlador.DeletarFilme(id);
                MenuHelper.ExibirMensagem(mensagem);
            }
            else
            {
                MenuHelper.ExibirMensagem("Operacao cancelada.");
            }

            MenuHelper.Pausar();
        }

        private void ExibirFilmesTabela(List<Filme> filmes)
        {
            Console.WriteLine("\n{0,-4} {1,-30} {2,-12} {3,-15} {4,-6}", "ID", "Titulo", "Duracao", "Genero", "3D");
            Console.WriteLine(new string('-', 80));
            foreach (var filme in filmes)
            {
                Console.WriteLine("{0,-4} {1,-30} {2,-12} {3,-15} {4,-6}", filme.Id, filme.Titulo, filme.Duracao + "min", filme.Genero, (filme.Eh3D ? "Sim" : "Não"));
            }
        }
    }
}
