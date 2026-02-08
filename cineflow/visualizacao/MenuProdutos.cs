using cineflow.controladores;
using cineflow.modelos;
using cineflow.enumeracoes;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuProdutos
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuProdutos(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Produtos");
                MenuHelper.MostrarOpcoes(
                    "Criar Produto",
                    "Listar Produtos",
                    "Buscar Produto por Nome",
                    "Obter Detalhes de Produto",
                    "Atualizar Produto",
                    "Adicionar ao Estoque",
                    "Reduzir Estoque",
                    "Listar Produtos com Estoque Baixo",
                    "Ver Alertas de Estoque",
                    "Deletar Produto");

                var opcao = MenuHelper.LerOpcaoInteira(0, 10);

                switch (opcao)
                {
                    case 1:
                        CriarProduto();
                        break;
                    case 2:
                        ListarProdutos();
                        break;
                    case 3:
                        BuscarProdutoPorNome();
                        break;
                    case 4:
                        ObterDetalhesProduto();
                        break;
                    case 5:
                        AtualizarProduto();
                        break;
                    case 6:
                        AdicionarEstoque();
                        break;
                    case 7:
                        ReduzirEstoque();
                        break;
                    case 8:
                        ListarProdutosEstoqueBaixo();
                        break;
                    case 9:
                        ListarAlertasEstoque();
                        break;
                    case 10:
                        DeletarProduto();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // CRIAR - 
        private void CriarProduto()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Criar Produto");

            try
            {
                var nome = MenuHelper.LerTextoNaoVazio("Nome do Produto: ");
                var descricao = MenuHelper.LerTextoNaoVazio("Descricao: ");

                Console.WriteLine("Categoria do Produto:");
                MenuHelper.MostrarOpcoes("Bebida", "Alimento", "Doce", "Tematico", "Brinde", "Poster", "Cortesia");
                var categoriaOpcao = MenuHelper.LerOpcaoInteira(1, 7);
                var categoria = (CategoriaProduto)categoriaOpcao;

                bool ehCortesia = categoria == CategoriaProduto.Cortesia || MenuHelper.Confirmar("Eh cortesia?");
                var preco = ehCortesia ? 0m : MenuHelper.LerDecimal("Preco: ", 0m, 1000m);
                var estoqueAtual = MenuHelper.LerInteiro("Estoque Inicial: ", 0, 10000);
                var estoqueMinimo = MenuHelper.LerInteiro("Estoque Mínimo: ", 0, 1000);

                bool ehTematico = categoria == CategoriaProduto.Tematico || MenuHelper.Confirmar("Produto tematico?");
                string? temaFilme = null;
                if (ehTematico)
                {
                    temaFilme = MenuHelper.LerTextoNaoVazio("Tema do filme: ");
                }

                bool exclusivoPreEstreia = MenuHelper.Confirmar("Exclusivo pre-estreia?");

                var produto = new ProdutoAlimento(0, nome, descricao, categoria, (float)preco, estoqueAtual, estoqueMinimo,
                    ehTematico, temaFilme, ehCortesia, exclusivoPreEstreia);
                var (sucesso, mensagem) = administradorControlador.ProdutoControlador.CriarProduto(produto);

                MenuHelper.ExibirMensagem(mensagem);
            }
            catch (Exception ex)
            {
                MenuHelper.ExibirMensagem($"Erro: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ListarProdutos()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Produtos");

            var (produtos, mensagem) = administradorControlador.ProdutoControlador.ListarProdutos();
            MenuHelper.ExibirMensagem(mensagem);

            if (produtos.Count > 0)
            {
                ExibirProdutosTabela(produtos);
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void BuscarProdutoPorNome()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Buscar Produto por Nome");

            var nome = MenuHelper.LerTextoNaoVazio("Nome do Produto: ");
            var (produtos, mensagem) = administradorControlador.ProdutoControlador.BuscarPorNome(nome);

            MenuHelper.ExibirMensagem(mensagem);

            if (produtos.Count > 0)
            {
                ExibirProdutosTabela(produtos);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void ObterDetalhesProduto()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Obter Detalhes de Produto");

            var id = MenuHelper.LerInteiro("ID do Produto: ", 1, 999999);
            var (produto, mensagem) = administradorControlador.ProdutoControlador.ObterProduto(id);

            MenuHelper.ExibirMensagem(mensagem);

            if (produto != null)
            {
                Console.WriteLine($"\nID: {produto.Id}");
                Console.WriteLine($"Nome: {produto.Nome}");
                Console.WriteLine($"Descricao: {produto.Descricao}");
                Console.WriteLine($"Preco: {FormatadorMoeda.Formatar(produto.Preco)}");
                Console.WriteLine($"Estoque Atual: {produto.EstoqueAtual}");
                Console.WriteLine($"Estoque Mínimo: {produto.EstoqueMinimo}");
                Console.WriteLine($"Categoria: {produto.Categoria}");
            }

            MenuHelper.Pausar();
        // ATUALIZAR - 
        }

        private void AtualizarProduto()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Atualizar Produto");

            var id = MenuHelper.LerInteiro("ID do Produto: ", 1, 999999);

            var nome = MenuHelper.LerTextoOpcional("Novo Nome (ou deixe em branco): ");

            var descricao = MenuHelper.LerTextoOpcional("Nova Descricao (ou deixe em branco): ");

            var precoStr = MenuHelper.LerTextoOpcional("Novo Preco (ou deixe em branco): ");
            float? preco = null;
            if (!string.IsNullOrWhiteSpace(precoStr) && decimal.TryParse(precoStr, out var p))
            {
                preco = (float)p;
            }

            Console.WriteLine("Nova Categoria (ou ENTER para manter):");
            MenuHelper.MostrarOpcoes("Bebida", "Alimento", "Doce", "Tematico", "Brinde", "Poster", "Cortesia");
            var categoriaStr = Console.ReadLine();
            CategoriaProduto? categoria = null;
            if (!string.IsNullOrWhiteSpace(categoriaStr) && int.TryParse(categoriaStr, out int catOpcao) && catOpcao >= 1 && catOpcao <= 7)
            {
                categoria = (CategoriaProduto)catOpcao;
            }

            bool? ehTematico = null;
            bool? ehCortesia = null;
            bool? exclusivoPreEstreia = null;
            string? temaFilme = null;

            if (MenuHelper.Confirmar("Alterar flags especiais?") )
            {
                ehTematico = MenuHelper.Confirmar("Produto tematico?");
                if (ehTematico.Value)
                {
                    temaFilme = MenuHelper.LerTextoNaoVazio("Tema do filme: ");
                }

                ehCortesia = MenuHelper.Confirmar("Eh cortesia?");
                exclusivoPreEstreia = MenuHelper.Confirmar("Exclusivo pre-estreia?");
            }

            var (sucesso, mensagem) = administradorControlador.ProdutoControlador.AtualizarProduto(
                id, nome, descricao, preco, null, ehTematico, temaFilme, ehCortesia, exclusivoPreEstreia, categoria);

            MenuHelper.ExibirMensagem(mensagem);
            MenuHelper.Pausar();
        // ATUALIZAR - 
        }

        private void AdicionarEstoque()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Adicionar ao Estoque");

            var id = MenuHelper.LerInteiro("ID do Produto: ", 1, 999999);
            var quantidade = MenuHelper.LerInteiro("Quantidade a Adicionar: ", 1, 10000);

            var (sucesso, mensagem) = administradorControlador.ProdutoControlador.AdicionarEstoque(id, quantidade);

            MenuHelper.ExibirMensagem(mensagem);
            MenuHelper.Pausar();
        // ATUALIZAR - 
        }

        private void ReduzirEstoque()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Reduzir Estoque");

            var id = MenuHelper.LerInteiro("ID do Produto: ", 1, 999999);
            var quantidade = MenuHelper.LerInteiro("Quantidade a Reduzir: ", 1, 10000);

            var (sucesso, mensagem) = administradorControlador.ProdutoControlador.ReduzirEstoque(id, quantidade);

            MenuHelper.ExibirMensagem(mensagem);
        // LER - 
            MenuHelper.Pausar();
        }

        private void ListarProdutosEstoqueBaixo()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Produtos com Estoque Baixo");

            var (produtos, mensagem) = administradorControlador.ProdutoControlador.ListarProdutosEstoqueBaixo();
            MenuHelper.ExibirMensagem(mensagem);

            if (produtos.Count > 0)
            {
                ExibirProdutosTabela(produtos);
            }
// DELETAR - 
        
            MenuHelper.Pausar();
        }

        private void ListarAlertasEstoque()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Alertas de Estoque");

            var (alertas, mensagem) = administradorControlador.ProdutoControlador.ListarAlertasEstoque();
            MenuHelper.ExibirMensagem(mensagem);
            if (alertas.Count > 0)
            {
                foreach (var alerta in alertas)
                {
                    Console.WriteLine($"- {alerta}");
                }
            }

            MenuHelper.Pausar();
        }

        private void DeletarProduto()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Deletar Produto");

            var id = MenuHelper.LerInteiro("ID do Produto: ", 1, 999999);

            Console.Write("Tem certeza que deseja deletar? (sim/nao): ");
            var confirmacao = Console.ReadLine();

            if (confirmacao?.ToLower() == "sim")
            {
                var (sucesso, mensagem) = administradorControlador.ProdutoControlador.DeletarProduto(id);
                MenuHelper.ExibirMensagem(mensagem);
            }
            else
            {
                MenuHelper.ExibirMensagem("Operacao cancelada.");
            }

            MenuHelper.Pausar();
        }

        private void ExibirProdutosTabela(List<ProdutoAlimento> produtos)
        {
            Console.WriteLine("\n{0,-4} {1,-20} {2,-10} {3,-10} {4,-12} {5,-10} {6,-12}", "ID", "Nome", "Preco", "Estoque", "Categoria", "Tematico", "Pre-Estreia");
            Console.WriteLine(new string('-', 95));
            foreach (var produto in produtos)
            {
                var tematico = produto.EhTematico ? (produto.TemaFilme ?? "Sim") : "Nao";
                var pre = produto.ExclusivoPreEstreia ? "Sim" : "Nao";
                Console.WriteLine("{0,-4} {1,-20} {2,-10} {3,-10} {4,-12} {5,-10} {6,-12}", produto.Id, Truncar(produto.Nome, 18), FormatadorMoeda.Formatar(produto.Preco), produto.EstoqueAtual, produto.Categoria, Truncar(tematico, 10), pre);
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
