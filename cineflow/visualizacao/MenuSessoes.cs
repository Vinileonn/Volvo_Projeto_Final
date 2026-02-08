using cineflow.controladores;
using cineflow.modelos;
using cineflow.utilitarios;
using cineflow.enumeracoes;

namespace cineflow.menus
{
    public class MenuSessoes
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuSessoes(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Sessoes");
                MenuHelper.MostrarOpcoes(
                    "Criar Sessao",
                    "Listar Sessoes",
                    "Listar Sessoes por Filme",
                    "Listar Sessoes por Sala",
                    "Obter Detalhes de Sessao",
                    "Atualizar Sessao",
                    "Deletar Sessao");

                var opcao = MenuHelper.LerOpcaoInteira(0, 7);

                switch (opcao)
                {
                    case 1:
                        CriarSessao();
                        break;
                    case 2:
                        ListarSessoes();
                        break;
                    case 3:
                        ListarSessoesPorFilme();
                        break;
                    case 4:
                        ListarSessoesPorSala();
                        break;
                    case 5:
                        ObterDetalhesSessao();
                        break;
                    case 6:
                        AtualizarSessao();
                        break;
                    case 7:
                        DeletarSessao();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // CRIAR - 
        private void CriarSessao()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Criar Sessao");

            try
            {
                var (filmes, mensagemFilmes) = administradorControlador.FilmeControlador.ListarFilmes();
                if (filmes.Count == 0)
                {
                    MenuHelper.ExibirMensagem("Nenhum filme disponivel.");
                    MenuHelper.Pausar();
                    return;
                }

                Console.WriteLine("\nFilmes Disponíveis:");
                ExibirFilmesTabela(filmes);

                var filmeId = MenuHelper.LerInteiro("ID do Filme: ", 1, filmes.Max(f => f.Id));
                var filme = filmes.FirstOrDefault(f => f.Id == filmeId);
                if (filme == null)
                {
                    MenuHelper.ExibirMensagem("Filme não encontrado.");
                    MenuHelper.Pausar();
                    return;
                }

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

                Console.Write("Data/Hora (formato dd/MM/yyyy HH:mm): ");
                var dataHoraStr = Console.ReadLine();
                if (!DateTime.TryParseExact(dataHoraStr, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out var dataHora))
                {
                    MenuHelper.ExibirMensagem("Formato de data/hora inválido.");
                    MenuHelper.Pausar();
                    return;
                }

                var precoBase = MenuHelper.LerDecimal("Preço Base (em reais): ", 0m, 1000m);

                var tipoSessao = LerTipoSessao();
                string? nomeEvento = null;
                string? parceiro = null;

                if (tipoSessao == TipoSessao.Evento)
                {
                    nomeEvento = MenuHelper.LerTextoNaoVazio("Nome do evento: ");
                    parceiro = MenuHelper.LerTextoOpcional("Parceiro (opcional): ");
                }

                var idioma = LerIdiomaSessao();

                var sessao = new Sessao(0, dataHora, (float)precoBase, filme, sala, tipoSessao, nomeEvento, parceiro, idioma);
                var (sucesso, mensagem) = administradorControlador.SessaoControlador.CriarSessao(sessao);

                MenuHelper.ExibirMensagem(mensagem);
            }
            catch (Exception ex)
            {
                MenuHelper.ExibirMensagem($"Erro: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ListarSessoes()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Sessoes");

            var (sessoes, mensagem) = administradorControlador.SessaoControlador.ListarSessoes();
            MenuHelper.ExibirMensagem(mensagem);

            if (sessoes.Count > 0)
            {
                ExibirSessoesTabela(sessoes);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void ListarSessoesPorFilme()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Sessoes por Filme");

            var (filmes, mensagem) = administradorControlador.FilmeControlador.ListarFilmes();
            if (filmes.Count == 0)
            {
                MenuHelper.ExibirMensagem("Nenhum filme disponivel.");
                MenuHelper.Pausar();
                return;
            }

            ExibirFilmesTabela(filmes);

            var filmeId = MenuHelper.LerInteiro("ID do Filme: ", 1, filmes.Max(f => f.Id));
            var (sessoes, mensagemSessoes) = administradorControlador.SessaoControlador.ListarSessoesPorFilme(filmeId);

            MenuHelper.ExibirMensagem(mensagemSessoes);

            if (sessoes.Count > 0)
            {
                ExibirSessoesTabela(sessoes);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void ListarSessoesPorSala()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Sessoes por Sala");

            var (salas, mensagem) = administradorControlador.SalaControlador.ListarSalas();
            if (salas.Count == 0)
            {
                MenuHelper.ExibirMensagem("Nenhuma sala disponivel.");
                MenuHelper.Pausar();
                return;
            }

            ExibirSalasTabela(salas);

            var salaId = MenuHelper.LerInteiro("ID da Sala: ", 1, salas.Max(s => s.Id));
            var (sessoes, mensagemSessoes) = administradorControlador.SessaoControlador.ListarSessoesPorSala(salaId);

            MenuHelper.ExibirMensagem(mensagemSessoes);

            if (sessoes.Count > 0)
            {
                ExibirSessoesTabela(sessoes);
            }

            MenuHelper.Pausar();
        // LER - 
        }

        private void ObterDetalhesSessao()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Obter Detalhes de Sessao");

            var id = MenuHelper.LerInteiro("ID da Sessao: ", 1, 999999);
            var (sessao, mensagem) = administradorControlador.SessaoControlador.ObterSessao(id);

            MenuHelper.ExibirMensagem(mensagem);

            if (sessao != null)
            {
                Console.WriteLine($"\nID: {sessao.Id}");
                var filmeTitulo = sessao.Filme != null ? sessao.Filme.Titulo : "Não informado";
                Console.WriteLine($"Filme: {filmeTitulo}");
                var salaNome = sessao.Sala != null ? sessao.Sala.Nome : "Não informado";
                Console.WriteLine($"Sala: {salaNome}");
                Console.WriteLine($"Data/Hora: {sessao.DataHorario:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Preco Base: {FormatadorMoeda.Formatar(sessao.PrecoBase)}");
                Console.WriteLine($"Preço Final: {FormatadorMoeda.Formatar(sessao.PrecoFinal)}");
                Console.WriteLine($"Ingressos Vendidos: {sessao.Ingressos.Count}");
                Console.WriteLine($"Tipo: {sessao.Tipo}");
                Console.WriteLine($"Idioma: {sessao.Idioma}");
                if (!string.IsNullOrWhiteSpace(sessao.NomeEvento))
                {
                    Console.WriteLine($"Evento: {sessao.NomeEvento}");
                }
                if (!string.IsNullOrWhiteSpace(sessao.Parceiro))
                {
                    Console.WriteLine($"Parceiro: {sessao.Parceiro}");
                }
            }

            MenuHelper.Pausar();
        // ATUALIZAR - 
        }

        private void AtualizarSessao()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Atualizar Sessao");

            var id = MenuHelper.LerInteiro("ID da Sessao: ", 1, 999999);

            Console.Write("Nova Data/Hora (formato dd/MM/yyyy HH:mm, ou deixe em branco): ");
            var dataHoraStr = Console.ReadLine();
            DateTime? novaDataHora = null;
            if (!string.IsNullOrWhiteSpace(dataHoraStr))
            {
                if (DateTime.TryParseExact(dataHoraStr, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out var dh))
                {
                    novaDataHora = dh;
                }
                else
                {
                    MenuHelper.ExibirMensagem("Formato inválido.");
                    MenuHelper.Pausar();
                    return;
                }
            }

            var precoStr = MenuHelper.LerTextoOpcional("Novo Preco Base (ou deixe em branco): ");
            decimal? novoPreco = null;
            if (!string.IsNullOrWhiteSpace(precoStr))
            {
                if (decimal.TryParse(precoStr, out var p))
                {
                    novoPreco = p;
                }
            }

            var tipoSessao = LerTipoSessaoOpcional();
            string? nomeEvento = null;
            string? parceiro = null;
            if (tipoSessao.HasValue && tipoSessao.Value == TipoSessao.Evento)
            {
                nomeEvento = MenuHelper.LerTextoNaoVazio("Nome do evento: ");
                parceiro = MenuHelper.LerTextoOpcional("Parceiro (opcional): ");
            }

            var idioma = LerIdiomaSessaoOpcional();

            var (sucesso, mensagem) = administradorControlador.SessaoControlador.AtualizarSessao(
                id, novaDataHora, novoPreco != null ? (float?)novoPreco : null, null, null, tipoSessao, nomeEvento, parceiro,
                idioma);

            MenuHelper.ExibirMensagem(mensagem);
        // DELETAR - 
            MenuHelper.Pausar();
        }

        private void DeletarSessao()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Deletar Sessao");

            var id = MenuHelper.LerInteiro("ID da Sessao: ", 1, 999999);

            Console.Write("Tem certeza que deseja deletar? (sim/nao): ");
            var confirmacao = Console.ReadLine();

            if (confirmacao?.ToLower() == "sim")
            {
                var (sucesso, mensagem) = administradorControlador.SessaoControlador.DeletarSessao(id);
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

        private void ExibirSalasTabela(List<Sala> salas)
        {
            Console.WriteLine("\n{0,-4} {1,-25} {2,-12} {3,-20} {4,-8}", "ID", "Nome", "Capacidade", "Cinema", "Tipo");
            Console.WriteLine(new string('-', 80));
            foreach (var sala in salas)
            {
                Console.WriteLine("{0,-4} {1,-25} {2,-12} {3,-20} {4,-8}", sala.Id, sala.Nome, sala.Capacidade, (sala.Cinema != null ? sala.Cinema.Nome : "Não informado"), sala.Tipo);
            }
        }

        private void ExibirSessoesTabela(List<Sessao> sessoes)
        {
            Console.WriteLine("\n{0,-4} {1,-22} {2,-16} {3,-18} {4,-10} {5,-10} {6,-10}", "ID", "Filme", "Sala", "Data/Hora", "Preço", "Tipo", "Idioma");
            Console.WriteLine(new string('-', 95));
            foreach (var sessao in sessoes)
            {
                Console.WriteLine("{0,-4} {1,-22} {2,-16} {3,-18} {4,-10} {5,-10} {6,-10}", sessao.Id,
                    (sessao.Filme != null ? Truncar(sessao.Filme.Titulo, 22) : "N/A"),
                    (sessao.Sala != null ? Truncar(sessao.Sala.Nome, 14) : "N/A"),
                    sessao.DataHorario.ToString("dd/MM/yyyy HH:mm"),
                    FormatadorMoeda.Formatar(sessao.PrecoFinal),
                    sessao.Tipo,
                    sessao.Idioma);
            }
        }

        private static TipoSessao LerTipoSessao()
        {
            Console.WriteLine("Tipo de Sessao:");
            MenuHelper.MostrarOpcoes("Regular", "Pre-estreia", "Especial bebe", "Especial pet", "Matine", "Evento");
            var opcao = MenuHelper.LerOpcaoInteira(1, 6);
            return (TipoSessao)opcao;
        }

        private static TipoSessao? LerTipoSessaoOpcional()
        {
            Console.WriteLine("Tipos: 1-Regular, 2-Pre-estreia, 3-Especial bebe, 4-Especial pet, 5-Matine, 6-Evento");
            Console.Write("Tipo de sessao (1-6, ENTER para manter): ");
            var entrada = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entrada))
            {
                return null;
            }

            if (int.TryParse(entrada, out int opcao) && opcao >= 1 && opcao <= 6)
            {
                return (TipoSessao)opcao;
            }

            Console.WriteLine("Opcao invalida. Mantendo tipo atual.");
            return null;
        }

        private static IdiomaSessao LerIdiomaSessao()
        {
            Console.WriteLine("Idioma da sessao:");
            MenuHelper.MostrarOpcoes("Dublado", "Legendado");
            var opcao = MenuHelper.LerOpcaoInteira(1, 2);
            return opcao == 1 ? IdiomaSessao.Dublado : IdiomaSessao.Legendado;
        }

        private static IdiomaSessao? LerIdiomaSessaoOpcional()
        {
            Console.Write("Idioma (1-Dublado, 2-Legendado, ENTER para manter): ");
            var entrada = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entrada))
            {
                return null;
            }

            if (!int.TryParse(entrada, out int opcao) || (opcao != 1 && opcao != 2))
            {
                Console.WriteLine("Opcao invalida. Mantendo idioma atual.");
                return null;
            }

            return opcao == 1 ? IdiomaSessao.Dublado : IdiomaSessao.Legendado;
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
