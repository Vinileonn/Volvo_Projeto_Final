using cineflow.controladores;
using cineflow.modelos;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuRelatorios
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuRelatorios(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Relatorios e Estatisticas");
                MenuHelper.MostrarOpcoes(
                    "Relatorio de Ingressos",
                    "Relatorio de Sessoes",
                    "Relatorio de Produtos/Pedidos",
                    "Relatorio Geral");

                var opcao = MenuHelper.LerOpcaoInteira(0, 4);

                switch (opcao)
                {
                    case 1:
                        RelatorioIngressos();
                        break;
                    case 2:
                        RelatorioSessoes();
                        break;
                    case 3:
                        RelatorioProdutosPedidos();
                        break;
                    case 4:
                        RelatorioGeral();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // LER - 
        private void RelatorioIngressos()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Relatorio de Ingressos");

            var (totalVendidos, msg1) = administradorControlador.RelatorioControlador.TotalIngressosVendidos();
            var (receitaIngressos, msg2) = administradorControlador.RelatorioControlador.ReceitaTotalIngressos();
            var (ingressosPorFilme, msg3) = administradorControlador.RelatorioControlador.IngressosPorFilme();

            Console.WriteLine($"Total de Ingressos Vendidos: {totalVendidos}");
            Console.WriteLine($"Receita Total de Ingressos: {FormatadorMoeda.Formatar(receitaIngressos)}");

            if (ingressosPorFilme.Count > 0)
            {
                Console.WriteLine("\nIngressos por Filme:");
                foreach (var kvp in ingressosPorFilme)
                {
                    Console.WriteLine($"  - {kvp.Key}: {kvp.Value} ingressos");
                }
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void RelatorioSessoes()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Relatorio de Sessoes");

            var top = MenuHelper.LerInteiro("Quantas sessoes top deseja ver? ", 1, 100);
            var (sessoesDados, msg) = administradorControlador.RelatorioControlador.SessoesComMaiorOcupacao(top);

            Console.WriteLine("\nSessoes com Maior Ocupacao:");
            if (sessoesDados.Count > 0)
            {
                // Extrair apenas as sessões da tupla
                var sessoesList = sessoesDados.Select(s => s.sessao).ToList();
                ExibirSessoesTabela(sessoesList);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void RelatorioProdutosPedidos()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Relatorio de Produtos/Pedidos");

            var (receitaPedidos, msg1) = administradorControlador.RelatorioControlador.ReceitaTotalPedidos();
            var (produtosMaisVendidos, msg2) = administradorControlador.RelatorioControlador.ProdutosMaisVendidos();
            var (produtosEstoqueBaixo, msg3) = administradorControlador.RelatorioControlador.ProdutosEstoqueBaixo();

            Console.WriteLine($"\nReceita Total de Pedidos: {FormatadorMoeda.Formatar(receitaPedidos)}");

            Console.WriteLine("\nProdutos Mais Vendidos:");
            if (produtosMaisVendidos.Count > 0)
            {
                foreach (var kvp in produtosMaisVendidos)
                {
                    Console.WriteLine($"  - {kvp.Key}: {kvp.Value} unidades");
                }
            }

            Console.WriteLine("\nProdutos com Estoque Baixo:");
            if (produtosEstoqueBaixo.Count > 0)
            {
                ExibirProdutosTabela(produtosEstoqueBaixo);
            }

            MenuHelper.Pausar();
        }
// LER - 
        
        private void RelatorioGeral()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Relatorio Geral");

            var (receitaGeral, msg1) = administradorControlador.RelatorioControlador.ReceitaTotalGeral();
            var (taxaOcupacao, msg2) = administradorControlador.RelatorioControlador.TaxaOcupacaoMedia();

            Console.WriteLine($"Receita Total Geral: {FormatadorMoeda.Formatar((float)receitaGeral)}");
            Console.WriteLine($"Taxa de Ocupacao Media: {taxaOcupacao:F2}%");

            Console.Write("Digite uma data de inicio (formato dd/MM/yyyy, ou deixe em branco): ");
            var startStr = Console.ReadLine();
            var start = string.IsNullOrWhiteSpace(startStr) ? DateTime.Now.AddMonths(-1) : 
                       DateTime.TryParseExact(startStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var d) ? d : DateTime.Now.AddMonths(-1);

            Console.Write("Digite uma data de fim (formato dd/MM/yyyy, ou deixe em branco): ");
            var endStr = Console.ReadLine();
            var end = string.IsNullOrWhiteSpace(endStr) ? DateTime.Now :
                     DateTime.TryParseExact(endStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var d2) ? d2 : DateTime.Now;

            var (ingressos, receitaIngressos, pedidos, receitaPedidos, msgVendas) = administradorControlador.RelatorioControlador.VendasPorPeriodo(start, end);
            Console.WriteLine($"\nVendas no Periodo ({start:dd/MM/yyyy} a {end:dd/MM/yyyy}):");
            Console.WriteLine($"  Ingressos Vendidos: {ingressos}");
            Console.WriteLine($"  Receita de Ingressos: {FormatadorMoeda.Formatar((float)receitaIngressos)}");
            Console.WriteLine($"  Pedidos: {pedidos}");
            Console.WriteLine($"  Receita de Pedidos: {FormatadorMoeda.Formatar((float)receitaPedidos)}");

            MenuHelper.Pausar();
        }

        private void ExibirSessoesTabela(List<Sessao> sessoes)
        {
            Console.WriteLine("\n{0,-4} {1,-20} {2,-14} {3,-18} {4,-10} {5,-8} {6,-9} {7,-7}", "ID", "Filme", "Sala", "Data/Hora", "Preço", "Tipo", "Idioma", "Classif");
            Console.WriteLine(new string('-', 100));
            foreach (var sessao in sessoes)
            {
                Console.WriteLine("{0,-4} {1,-20} {2,-14} {3,-18} {4,-10} {5,-8} {6,-9} {7,-6}", sessao.Id,
                    (sessao.Filme != null ? Truncar(sessao.Filme.Titulo, 18) : "N/A"),
                    (sessao.Sala != null ? Truncar(sessao.Sala.Nome, 12) : "N/A"),
                    sessao.DataHorario.ToString("dd/MM/yyyy HH:mm"),
                    FormatadorMoeda.Formatar(sessao.PrecoFinal),
                    Truncar(sessao.Tipo.ToString(), 6),
                    Truncar(sessao.Idioma.ToString(), 7),
                    (sessao.Filme != null ? (int)sessao.Filme.Classificacao : 0));
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

        private void ExibirProdutosTabela(List<ProdutoAlimento> produtos)
        {
            Console.WriteLine("\n{0,-4} {1,-25} {2,-12} {3,-15} {4,-15}", "ID", "Nome", "Preço", "Estoque", "Categoria");
            Console.WriteLine(new string('-', 80));
            foreach (var produto in produtos)
            {
                Console.WriteLine("{0,-4} {1,-25} {2,-12} {3,-15} {4,-15}", produto.Id, produto.Nome, FormatadorMoeda.Formatar(produto.Preco), produto.EstoqueAtual, produto.Categoria);
            }
        }
    }
}
