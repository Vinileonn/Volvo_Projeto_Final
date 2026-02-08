using cineflow.controladores;
using cineflow.modelos;
using cineflow.modelos.UsuarioModelo;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuPrincipal
    {
        private readonly ClienteControlador clienteControlador;
        private readonly AutenticacaoControlador autenticacaoControlador;
        private readonly ProdutoControlador produtoControlador;
        private readonly List<ItemPedidoAlimento> carrinho;

        public MenuPrincipal(
            ClienteControlador clienteControlador,
            AutenticacaoControlador autenticacaoControlador,
            ProdutoControlador produtoControlador)
        {
            this.clienteControlador = clienteControlador;
            this.autenticacaoControlador = autenticacaoControlador;
            this.produtoControlador = produtoControlador;
            carrinho = new List<ItemPedidoAlimento>();
        }

        public MenuPrincipalResultado Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Menu Principal");
                MenuHelper.MostrarOpcoes(
                    "Visualizar cartaz",
                    "Visualizar sessoes",
                    "Visualizar produtos",
                    "Adicionar ao carrinho",
                    "Ver carrinho",
                    "Login (cliente/admin)",
                    "Cadastro de cliente");

                var opcao = MenuHelper.LerOpcaoInteira(0, 7);

                switch (opcao)
                {
                    case 1:
                        VisualizarCartaz();
                        break;
                    case 2:
                        VisualizarSessoes();
                        break;
                    case 3:
                        VisualizarProdutos();
                        break;
                    case 4:
                        AdicionarAoCarrinho();
                        break;
                    case 5:
                        VerCarrinho();
                        break;
                    case 6:
                    {
                        var resultadoLogin = FazerLogin();
                        if (resultadoLogin.UsuarioLogado != null)
                        {
                            return resultadoLogin;
                        }
                        break;
                    }
                    case 7:
                        CadastrarCliente();
                        break;
                    case 0:
                        return new MenuPrincipalResultado(null, true, carrinho);
                }
            }
        }

        // LER - 
        private void VisualizarCartaz()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Cartaz de Filmes");

            var (filmes, mensagem) = clienteControlador.ListarCartaz();
            MenuHelper.ExibirMensagem(mensagem);

            if (filmes.Count > 0)
            {
                ExibirFilmesTabela(filmes);
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void VisualizarSessoes()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Sessoes Disponiveis");
            MenuHelper.MostrarOpcoes(
                "Ver todas as sessoes",
                "Ver sessoes por filme");

            var opcao = MenuHelper.LerOpcaoInteira(0, 2);
            if (opcao == 0)
            {
                return;
            }

            if (opcao == 1)
            {
                var (sessoes, mensagem) = clienteControlador.ListarTodasSessoes();
                MenuHelper.ExibirMensagem(mensagem);
                if (sessoes.Count > 0)
                {
                    ExibirSessoesTabela(sessoes);
                }

                MenuHelper.Pausar();
                return;
            }

            var (filmes, mensagemFilmes) = clienteControlador.ListarCartaz();
            MenuHelper.ExibirMensagem(mensagemFilmes);
            if (filmes.Count == 0)
            {
                MenuHelper.Pausar();
                return;
            }

            ExibirFilmesTabela(filmes);
            int maxId = filmes.Max(f => f.Id);
            int filmeId = MenuHelper.LerInteiro("Informe o ID do filme: ", 1, maxId);

            if (!filmes.Any(f => f.Id == filmeId))
            {
                MenuHelper.ExibirMensagem("Filme nao encontrado.");
                MenuHelper.Pausar();
                return;
            }

            var (sessoesPorFilme, mensagemSessoes) = clienteControlador.ListarSessoesPorFilme(filmeId);
            MenuHelper.ExibirMensagem(mensagemSessoes);
            if (sessoesPorFilme.Count > 0)
            {
                ExibirSessoesTabela(sessoesPorFilme);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void VisualizarProdutos()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Produtos e Combos");

            var (produtos, mensagem) = clienteControlador.ListarProdutosAlimento();
            MenuHelper.ExibirMensagem(mensagem);

            if (produtos.Count > 0)
            {
                ExibirProdutosTabela(produtos);
            }

            MenuHelper.Pausar();
        }
// CRIAR - 
        
        private void AdicionarAoCarrinho()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Adicionar ao Carrinho");

            var (produtos, mensagem) = clienteControlador.ListarProdutosAlimento();
            MenuHelper.ExibirMensagem(mensagem);

            if (produtos.Count == 0)
            {
                MenuHelper.Pausar();
                return;
            }

            ExibirProdutosTabela(produtos);

            int maxId = produtos.Max(p => p.Id);
            int produtoId = MenuHelper.LerInteiro("Informe o ID do produto: ", 1, maxId);

            var produto = produtos.FirstOrDefault(p => p.Id == produtoId);
            if (produto == null)
            {
                MenuHelper.ExibirMensagem("Produto nao encontrado.");
                MenuHelper.Pausar();
                return;
            }

            int quantidade = MenuHelper.LerInteiro("Quantidade: ", 1, 999);

            var (disponivel, mensagemDisponibilidade) = produtoControlador.VerificarDisponibilidade(produtoId, quantidade);
            if (!disponivel)
            {
                MenuHelper.ExibirMensagem(mensagemDisponibilidade);
                MenuHelper.Pausar();
                return;
            }

            var itemExistente = carrinho.FirstOrDefault(i => i.Produto?.Id == produtoId);
            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                carrinho.Add(new ItemPedidoAlimento(0, produto, quantidade, produto.Preco));
            }

            MenuHelper.ExibirMensagem("Produto adicionado ao carrinho.");
            MenuHelper.Pausar();
        }
// LER - 
        
        private void VerCarrinho()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Carrinho");

            if (carrinho.Count == 0)
            {
                MenuHelper.ExibirMensagem("Carrinho vazio.");
                MenuHelper.Pausar();
                return;
            }

            ExibirCarrinhoTabela(carrinho);
            var total = carrinho.Sum(i => i.Preco * i.Quantidade);
            Console.WriteLine($"Total: {FormatadorMoeda.Formatar(total)}");
            Console.WriteLine("Para finalizar, faca login ou cadastre-se.");
            MenuHelper.Pausar();
        // AUTENTICACAO - 
        }

        private MenuPrincipalResultado FazerLogin()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Login");

            string email = MenuHelper.LerTextoNaoVazio("Email: ");
            string senha = MenuHelper.LerTextoNaoVazio("Senha: ");

            var (usuario, mensagem) = autenticacaoControlador.Autenticar(email, senha);
            MenuHelper.ExibirMensagem(mensagem);

            if (usuario == null)
            {
                MenuHelper.Pausar();
                return new MenuPrincipalResultado(null, false, carrinho);
            }

            return new MenuPrincipalResultado(usuario, false, carrinho);
        // CRIAR - 
        }

        private void CadastrarCliente()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Cadastro de Cliente");

            string nome = MenuHelper.LerTextoNaoVazio("Nome: ");
            string email = MenuHelper.LerTextoNaoVazio("Email: ");
            string senha = MenuHelper.LerTextoNaoVazio("Senha: ");
            string cpf = MenuHelper.LerTextoNaoVazio("CPF: ");
            string telefone = MenuHelper.LerTextoNaoVazio("Telefone: ");
            string endereco = MenuHelper.LerTextoNaoVazio("Endereco: ");
            DateTime dataNascimento = MenuHelper.LerData("Data de nascimento (dd/MM/yyyy): ");

            var cliente = new Cliente(0, nome, email, senha, cpf, telefone, endereco, dataNascimento);
            var (sucesso, mensagem) = autenticacaoControlador.RegistrarCliente(cliente);
            MenuHelper.ExibirMensagem(mensagem);

            if (sucesso)
            {
                MenuHelper.ExibirMensagem("Cadastro concluido. Faca login para continuar.");
            }

            MenuHelper.Pausar();
        }

        private static void ExibirFilmesTabela(List<Filme> filmes)
        {
            Console.WriteLine();
            Console.WriteLine($"{"ID",-4}{"Titulo",-32}{"Genero",-18}{"Duracao",-10}{"3D",-4}");
            Console.WriteLine(new string('-', 68));

            foreach (var filme in filmes)
            {
                string duracao = FormatarDuracao(filme.Duracao);
                Console.WriteLine($"{filme.Id,-4}{Truncar(filme.Titulo, 30),-32}{Truncar(filme.Genero, 16),-18}{duracao,-10}{(filme.Eh3D ? "Sim" : "Nao"),-4}");
            }

            Console.WriteLine();
        }

        private static void ExibirSessoesTabela(List<Sessao> sessoes)
        {
            Console.WriteLine();
            Console.WriteLine($"{"ID",-4}{"Filme",-28}{"Sala",-12}{"Data/Hora",-18}{"Preco",-10}");
            Console.WriteLine(new string('-', 72));

            foreach (var sessao in sessoes)
            {
                string dataHora = FormatadorData.FormatarDataComHora(sessao.DataHorario);
                Console.WriteLine($"{sessao.Id,-4}{Truncar(sessao.Filme.Titulo, 26),-28}{Truncar(sessao.Sala.Nome, 10),-12}{dataHora,-18}{FormatadorMoeda.Formatar(sessao.PrecoFinal),-10}");
            }

            Console.WriteLine();
        }

        private static void ExibirProdutosTabela(List<ProdutoAlimento> produtos)
        {
            Console.WriteLine();
            Console.WriteLine($"{"ID",-4}{"Produto",-28}{"Categoria",-14}{"Preco",-12}{"Estoque",-8}");
            Console.WriteLine(new string('-', 70));

            foreach (var produto in produtos)
            {
                string categoria = produto.Categoria?.ToString() ?? "-";
                Console.WriteLine($"{produto.Id,-4}{Truncar(produto.Nome, 26),-28}{Truncar(categoria, 12),-14}{FormatadorMoeda.Formatar(produto.Preco),-12}{produto.EstoqueAtual,-8}");
            }

            Console.WriteLine();
        }

        private static void ExibirCarrinhoTabela(List<ItemPedidoAlimento> itens)
        {
            Console.WriteLine();
            Console.WriteLine($"{"Item",-6}{"Produto",-28}{"Qtd",-6}{"Preco",-12}{"Subtotal",-12}");
            Console.WriteLine(new string('-', 70));

            int indice = 1;
            foreach (var item in itens)
            {
                Console.WriteLine($"{indice,-6}{Truncar(item.Produto?.Nome ?? "-", 26),-28}{item.Quantidade,-6}{FormatadorMoeda.Formatar(item.Preco),-12}{FormatadorMoeda.Formatar(item.Preco * item.Quantidade),-12}");
                indice++;
            }

            Console.WriteLine();
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

        private static string FormatarDuracao(int minutos)
        {
            int horas = minutos / 60;
            int resto = minutos % 60;
            return $"{horas}h {resto}m";
        }
    }

    public class MenuPrincipalResultado
    {
        public Usuario? UsuarioLogado { get; }
        public bool Encerrar { get; }
        public List<ItemPedidoAlimento> Carrinho { get; }

        public MenuPrincipalResultado(Usuario? usuarioLogado, bool encerrar, List<ItemPedidoAlimento> carrinho)
        {
            UsuarioLogado = usuarioLogado;
            Encerrar = encerrar;
            Carrinho = carrinho;
        }
    }
}
