using cineflow.controladores;
using cineflow.enumeracoes;
using cineflow.modelos;
using cineflow.modelos.IngressoModelo;
using cineflow.modelos.UsuarioModelo;
using cineflow.utilitarios;

namespace cineflow.visualizacao
{
    public class MenuCliente
    {
        private readonly ClienteControlador clienteControlador;
        private readonly PedidoControlador pedidoControlador;
        private readonly AutenticacaoControlador autenticacaoControlador;
        private List<ItemPedidoAlimento> carrinho = new List<ItemPedidoAlimento>();

        public MenuCliente(
            ClienteControlador clienteControlador,
            PedidoControlador pedidoControlador,
            AutenticacaoControlador autenticacaoControlador)
        {
            this.clienteControlador = clienteControlador;
            this.pedidoControlador = pedidoControlador;
            this.autenticacaoControlador = autenticacaoControlador;
        }

        public void Executar(Cliente cliente, List<ItemPedidoAlimento> carrinho)
        {
            this.carrinho = carrinho ?? new List<ItemPedidoAlimento>();

            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Menu Cliente");
                MenuHelper.MostrarOpcoes(
                    "Visualizar cartaz",
                    "Visualizar sessoes",
                    "Comprar ingresso",
                    "Ver meus ingressos",
                    "Gerenciar carrinho de produtos",
                    "Ver historico de pedidos",
                    "Alterar senha",
                    "Logout");

                var opcao = MenuHelper.LerOpcaoInteira(0, 8);

                switch (opcao)
                {
                    case 1:
                        VisualizarCartaz();
                        break;
                    case 2:
                        VisualizarSessoes();
                        break;
                    case 3:
                        ComprarIngresso(cliente);
                        break;
                    case 4:
                        VerIngressos(cliente);
                        break;
                    case 5:
                        GerenciarCarrinho(cliente);
                        break;
                    case 6:
                        VerHistoricoPedidos(cliente);
                        break;
                    case 7:
                        AlterarSenha(cliente);
                        break;
                    case 8:
                    case 0:
                        return;
                }
            }
        }

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

        private void ComprarIngresso(Cliente cliente)
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Comprar Ingresso");
            MenuHelper.MostrarOpcoes("Ingresso inteira", "Ingresso meia");

            var opcao = MenuHelper.LerOpcaoInteira(0, 2);
            if (opcao == 0)
            {
                return;
            }

            var sessao = SelecionarSessao();
            if (sessao == null)
            {
                MenuHelper.Pausar();
                return;
            }

            Console.WriteLine(sessao.Sala.VisualizarDisposicao());

            char fila = LerFila();
            int numero = MenuHelper.LerInteiro("Numero do assento: ", 1, 999);

            var formaPagamento = LerFormaPagamento();
            decimal valorPago = 0m;
            if (formaPagamento == FormaPagamento.Dinheiro)
            {
                valorPago = MenuHelper.LerDecimal("Valor pago: ", 0m, 100000m);
            }

            if (opcao == 1)
            {
                var (ingresso, mensagem) = clienteControlador.ComprarIngressoInteiro(
                    sessao,
                    cliente,
                    fila,
                    numero,
                    formaPagamento,
                    valorPago);

                MenuHelper.ExibirMensagem(mensagem);
                ExibirTrocoIngresso(ingresso);
                MenuHelper.Pausar();
                return;
            }

            string motivo = MenuHelper.LerTextoNaoVazio("Motivo da meia entrada: ");
            var (ingressoMeia, mensagemMeia) = clienteControlador.ComprarIngressoMeia(
                sessao,
                cliente,
                fila,
                numero,
                motivo,
                formaPagamento,
                valorPago);

            MenuHelper.ExibirMensagem(mensagemMeia);
            ExibirTrocoIngresso(ingressoMeia);
            MenuHelper.Pausar();
        }

        private void VerIngressos(Cliente cliente)
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Meus Ingressos");

            var (ingressos, mensagem) = clienteControlador.ListarIngressosDoCliente(cliente);
            MenuHelper.ExibirMensagem(mensagem);

            if (ingressos.Count > 0)
            {
                ExibirIngressosTabela(ingressos);
            }

            MenuHelper.Pausar();
        }

        private void GerenciarCarrinho(Cliente cliente)
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Carrinho de Produtos");
                MenuHelper.MostrarOpcoes(
                    "Ver produtos disponiveis",
                    "Limpar carrinho",
                    "Adicionar item ao carrinho",
                    "Ver carrinho",
                    "Remover item do carrinho",
                    "Finalizar pedido");

                var opcao = MenuHelper.LerOpcaoInteira(0, 6);

                switch (opcao)
                {
                    case 1:
                        VisualizarProdutos();
                        break;
                    case 2:
                        LimparCarrinho();
                        break;
                    case 3:
                        AdicionarItemCarrinho();
                        break;
                    case 4:
                        VerCarrinho();
                        break;
                    case 5:
                        RemoverItemCarrinho();
                        break;
                    case 6:
                        FinalizarPedido(cliente);
                        break;
                    case 0:
                        return;
                }
            }
        }

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

        private void LimparCarrinho()
        {
            if (carrinho.Count == 0)
            {
                MenuHelper.ExibirMensagem("Carrinho vazio.");
                MenuHelper.Pausar();
                return;
            }

            carrinho.Clear();
            MenuHelper.ExibirMensagem("Carrinho limpo com sucesso.");
            MenuHelper.Pausar();
        }

        private void AdicionarItemCarrinho()
        {
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
            if (quantidade > produto.EstoqueAtual)
            {
                MenuHelper.ExibirMensagem("Quantidade indisponivel em estoque.");
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

        private void VerCarrinho()
        {
            if (carrinho.Count == 0)
            {
                MenuHelper.ExibirMensagem("Carrinho vazio.");
                MenuHelper.Pausar();
                return;
            }

            ExibirCarrinhoTabela(carrinho);
            var total = carrinho.Sum(i => i.Preco * i.Quantidade);
            Console.WriteLine($"Total: {FormatadorMoeda.Formatar(total)}");
            MenuHelper.Pausar();
        }

        private void RemoverItemCarrinho()
        {
            if (carrinho.Count == 0)
            {
                MenuHelper.ExibirMensagem("Carrinho vazio.");
                MenuHelper.Pausar();
                return;
            }

            ExibirCarrinhoTabela(carrinho);
            int maxIndex = carrinho.Count;
            int itemIndex = MenuHelper.LerInteiro("Informe o numero do item: ", 1, maxIndex);

            carrinho.RemoveAt(itemIndex - 1);
            MenuHelper.ExibirMensagem("Item removido do carrinho.");
            MenuHelper.Pausar();
        }

        private void FinalizarPedido(Cliente cliente)
        {
            if (carrinho.Count == 0)
            {
                MenuHelper.ExibirMensagem("Carrinho vazio.");
                MenuHelper.Pausar();
                return;
            }

            var (pedido, mensagem) = clienteControlador.CriarPedidoAlimento(cliente);
            MenuHelper.ExibirMensagem(mensagem);
            if (pedido == null)
            {
                MenuHelper.Pausar();
                return;
            }

            foreach (var item in carrinho)
            {
                if (item.Produto == null)
                {
                    MenuHelper.ExibirMensagem("Produto invalido no carrinho.");
                    MenuHelper.Pausar();
                    return;
                }

                var resultadoItem = clienteControlador.AdicionarItemAoPedido(pedido.Id, item.Produto.Id, item.Quantidade);
                if (!resultadoItem.sucesso)
                {
                    MenuHelper.ExibirMensagem(resultadoItem.mensagem);
                    MenuHelper.Pausar();
                    return;
                }
            }

            var (pedidoAtualizado, mensagemPedido) = clienteControlador.ObterPedido(pedido.Id);
            MenuHelper.ExibirMensagem(mensagemPedido);
            if (pedidoAtualizado == null)
            {
                MenuHelper.Pausar();
                return;
            }

            ExibirPedidoResumo(pedidoAtualizado);
            var formaPagamento = LerFormaPagamento();
            decimal valorPago = 0m;
            if (formaPagamento == FormaPagamento.Dinheiro)
            {
                valorPago = MenuHelper.LerDecimal("Valor pago: ", 0m, 100000m);
            }

            var resultado = pedidoControlador.RegistrarPagamento(pedido.Id, formaPagamento, valorPago);
            MenuHelper.ExibirMensagem(resultado.mensagem);

            var (pedidoPago, mensagemPago) = clienteControlador.ObterPedido(pedido.Id);
            MenuHelper.ExibirMensagem(mensagemPago);
            if (pedidoPago != null && pedidoPago.ValorTroco > 0)
            {
                Console.WriteLine($"Troco: {FormatadorMoeda.Formatar(pedidoPago.ValorTroco)}");
            }

            if (resultado.sucesso)
            {
                carrinho.Clear();
            }

            MenuHelper.Pausar();
        }

        private void VerHistoricoPedidos(Cliente cliente)
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Historico de Pedidos");

            if (cliente.Pedidos.Count == 0)
            {
                MenuHelper.ExibirMensagem("Nenhum pedido encontrado.");
                MenuHelper.Pausar();
                return;
            }

            foreach (var pedido in cliente.Pedidos)
            {
                ExibirPedidoResumo(pedido);
            }

            MenuHelper.Pausar();
        }

        private void AlterarSenha(Cliente cliente)
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Alterar Senha");

            string senhaAtual = MenuHelper.LerTextoNaoVazio("Senha atual: ");
            string senhaNova = MenuHelper.LerTextoNaoVazio("Nova senha: ");

            var resultado = autenticacaoControlador.AlterarSenha(cliente.Id, senhaAtual, senhaNova);
            MenuHelper.ExibirMensagem(resultado.mensagem);
            MenuHelper.Pausar();
        }

        private Sessao? SelecionarSessao()
        {
            MenuHelper.MostrarOpcoes(
                "Ver todas as sessoes",
                "Ver sessoes por filme");

            var opcao = MenuHelper.LerOpcaoInteira(0, 2);
            if (opcao == 0)
            {
                return null;
            }

            if (opcao == 1)
            {
                var (sessoes, mensagem) = clienteControlador.ListarTodasSessoes();
                MenuHelper.ExibirMensagem(mensagem);
                if (sessoes.Count == 0)
                {
                    return null;
                }

                ExibirSessoesTabela(sessoes);
                return LerSessaoPorId(sessoes);
            }

            var (filmes, mensagemFilmes) = clienteControlador.ListarCartaz();
            MenuHelper.ExibirMensagem(mensagemFilmes);
            if (filmes.Count == 0)
            {
                return null;
            }

            ExibirFilmesTabela(filmes);
            int maxId = filmes.Max(f => f.Id);
            int filmeId = MenuHelper.LerInteiro("Informe o ID do filme: ", 1, maxId);

            if (!filmes.Any(f => f.Id == filmeId))
            {
                MenuHelper.ExibirMensagem("Filme nao encontrado.");
                return null;
            }

            var (sessoesPorFilme, mensagemSessoes) = clienteControlador.ListarSessoesPorFilme(filmeId);
            MenuHelper.ExibirMensagem(mensagemSessoes);
            if (sessoesPorFilme.Count == 0)
            {
                return null;
            }

            ExibirSessoesTabela(sessoesPorFilme);
            return LerSessaoPorId(sessoesPorFilme);
        }

        private Sessao? LerSessaoPorId(List<Sessao> sessoes)
        {
            int maxId = sessoes.Max(s => s.Id);
            int sessaoId = MenuHelper.LerInteiro("Informe o ID da sessao: ", 1, maxId);
            var sessao = sessoes.FirstOrDefault(s => s.Id == sessaoId);
            if (sessao == null)
            {
                MenuHelper.ExibirMensagem("Sessao nao encontrada.");
            }
            return sessao;
        }

        private static void ExibirTrocoIngresso(Ingresso? ingresso)
        {
            if (ingresso == null)
            {
                return;
            }

            if (ingresso.ValorTroco > 0)
            {
                Console.WriteLine($"Troco: {FormatadorMoeda.Formatar(ingresso.ValorTroco)}");
            }
        }

        private static char LerFila()
        {
            while (true)
            {
                string entrada = MenuHelper.LerTextoNaoVazio("Fila do assento (A-Z): ");
                char fila = char.ToUpperInvariant(entrada[0]);
                if (char.IsLetter(fila))
                {
                    return fila;
                }
                Console.WriteLine("Fila invalida.");
            }
        }

        private static FormaPagamento LerFormaPagamento()
        {
            Console.WriteLine();
            Console.WriteLine("Formas de pagamento:");
            var formas = Enum.GetValues<FormaPagamento>();
            for (int i = 0; i < formas.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {formas[i]}");
            }

            int opcao = MenuHelper.LerInteiro("Escolha a forma de pagamento: ", 1, formas.Length);
            return formas[opcao - 1];
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

        private static void ExibirIngressosTabela(List<Ingresso> ingressos)
        {
            Console.WriteLine();
            Console.WriteLine($"{"ID",-4}{"Filme",-28}{"Sessao",-18}{"Assento",-8}{"Tipo",-18}{"Preco",-10}");
            Console.WriteLine(new string('-', 90));

            foreach (var ingresso in ingressos)
            {
                string dataHora = FormatadorData.FormatarDataComHora(ingresso.Sessao.DataHorario);
                string preco = FormatadorMoeda.Formatar((decimal)ingresso.CalcularPreco(0f));
                Console.WriteLine($"{ingresso.Id,-4}{Truncar(ingresso.Sessao.Filme.Titulo, 26),-28}{dataHora,-18}{ingresso.Fila}{ingresso.Numero,-6}{Truncar(ingresso.ObterTipo(), 16),-18}{preco,-10}");
            }

            Console.WriteLine();
        }

        private static void ExibirPedidoResumo(PedidoAlimento pedido)
        {
            Console.WriteLine();
            Console.WriteLine($"Pedido {pedido.Id} - {FormatadorData.FormatarDataComHora(pedido.DataPedido)}");
            ExibirItensPedido(pedido.Itens);
            Console.WriteLine($"Total: {FormatadorMoeda.Formatar(pedido.ValorTotal)}");
            if (pedido.FormaPagamento.HasValue)
            {
                Console.WriteLine($"Pagamento: {pedido.FormaPagamento}");
            }
            Console.WriteLine();
        }

        private static void ExibirItensPedido(List<ItemPedidoAlimento> itens)
        {
            Console.WriteLine();
            Console.WriteLine($"{"Item",-6}{"Produto",-28}{"Qtd",-6}{"Preco",-12}{"Subtotal",-12}");
            Console.WriteLine(new string('-', 70));

            foreach (var item in itens)
            {
                Console.WriteLine($"{item.Id,-6}{Truncar(item.Produto?.Nome ?? "-", 26),-28}{item.Quantidade,-6}{FormatadorMoeda.Formatar(item.Preco),-12}{FormatadorMoeda.Formatar(item.Preco * item.Quantidade),-12}");
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
}
