using cineflow.modelos;
using cineflow.modelos.IngressoModelo;

namespace cineflow.servicos
{
    public class RelatorioServico
    {
        private readonly IngressoServico IngressoServico;
        private readonly SessaoServico SessaoServico;
        private readonly ProdutoAlimentoServico produtoService;
        private readonly PedidoAlimentoServico pedidoService;

        public RelatorioServico(IngressoServico IngressoServico, SessaoServico SessaoServico,
                                ProdutoAlimentoServico produtoService, PedidoAlimentoServico pedidoService)
        {
            this.IngressoServico = IngressoServico;
            this.SessaoServico = SessaoServico;
            this.produtoService = produtoService;
            this.pedidoService = pedidoService;
        }

        // Relatório de ingressos vendidos
        public int TotalIngressosVendidos()
        {
            return IngressoServico.ListarIngressos().Count;
        }

        // Receita total de ingressos
        public float ReceitaTotalIngressos()
        {
            var ingressos = IngressoServico.ListarIngressos();
            return ingressos.Sum(i => i.CalcularPreco(i.Sessao.PrecoFinal));
        }

        // Ingressos vendidos por filme
        public Dictionary<string, int> IngressosPorFilme()
        {
            var ingressos = IngressoServico.ListarIngressos();
            return ingressos
                .GroupBy(i => i.Sessao.Filme.Titulo)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Sessões com maior ocupação
        public List<(Sessao sessao, int ingressosVendidos, float percentualOcupacao)> SessoesComMaiorOcupacao(int top = 5)
        {
            var sessoes = SessaoServico.ListarSessoes();
            return sessoes
                .Select(s => (
                    sessao: s,
                    ingressosVendidos: s.Ingressos.Count,
                    percentualOcupacao: s.Sala.Capacidade > 0 ? (float)s.Ingressos.Count / s.Sala.Capacidade * 100 : 0
                ))
                .OrderByDescending(x => x.percentualOcupacao)
                .Take(top)
                .ToList();
        }

        // Receita total de pedidos
        public float ReceitaTotalPedidos()
        {
            var pedidos = pedidoService.ListarPedidos();
            return pedidos.Sum(p => p.ValorTotal);
        }

        // Produtos mais vendidos
        public Dictionary<string, int> ProdutosMaisVendidos()
        {
            var pedidos = pedidoService.ListarPedidos();
            return pedidos
                .SelectMany(p => p.Itens)
                .Where(i => i.Produto != null)
                .GroupBy(i => i.Produto!.Nome)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantidade));
        }

        // Produtos com estoque baixo
        public List<ProdutoAlimento> ProdutosEstoqueBaixo()
        {
            return produtoService.ListarProdutosEstoqueBaixo();
        }

        // Receita total geral (ingressos + pedidos)
        public float ReceitaTotalGeral()
        {
            return ReceitaTotalIngressos() + ReceitaTotalPedidos();
        }

        // Relatório de vendas por período
        public (int ingressos, float receitaIngressos, int pedidos, float receitaPedidos) VendasPorPeriodo(DateTime inicio, DateTime fim)
        {
            var ingressos = IngressoServico.ListarIngressos()
                .Where(i => i.DataCompra >= inicio && i.DataCompra <= fim)
                .ToList();

            var pedidos = pedidoService.ListarPedidos()
                .Where(p => p.DataPedido >= inicio && p.DataPedido <= fim)
                .ToList();

            return (
                ingressos: ingressos.Count,
                receitaIngressos: ingressos.Sum(i => i.CalcularPreco(i.Sessao.PrecoFinal)),
                pedidos: pedidos.Count,
                receitaPedidos: pedidos.Sum(p => p.ValorTotal)
            );
        }

        // Taxa de ocupação média das sessões
        public float TaxaOcupacaoMedia()
        {
            var sessoes = SessaoServico.ListarSessoes();
            if (sessoes.Count == 0)
            {
                return 0;
            }

            var ocupacoes = sessoes
                .Where(s => s.Sala.Capacidade > 0)
                .Select(s => (float)s.Ingressos.Count / s.Sala.Capacidade * 100);

            return ocupacoes.Any() ? ocupacoes.Average() : 0;
        }
    }
}





