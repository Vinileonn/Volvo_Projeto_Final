using cinecore.Models;

namespace cinecore.Services
{
    public class RelatorioServico
    {
        private readonly IngressoServico _ingressoServico;
        private readonly SessaoServico _sessaoServico;
        private readonly ProdutoAlimentoServico _produtoServico;
        private readonly PedidoAlimentoServico _pedidoServico;
        private const int DiasCartazPadrao = 7;

        public RelatorioServico(IngressoServico ingressoServico, SessaoServico sessaoServico,
                                ProdutoAlimentoServico produtoServico, PedidoAlimentoServico pedidoServico)
        {
            _ingressoServico = ingressoServico;
            _sessaoServico = sessaoServico;
            _produtoServico = produtoServico;
            _pedidoServico = pedidoServico;
        }

        // Relatório de ingressos vendidos
        public int TotalIngressosVendidos()
        {
            return _ingressoServico.ListarIngressos().Count;
        }

        // Receita total de ingressos
        public decimal ReceitaTotalIngressos()
        {
            var ingressos = _ingressoServico.ListarIngressos();
            return ingressos.Sum(i => i.CalcularPreco(i.Sessao?.PrecoFinal ?? 0));
        }

        // Ingressos vendidos por filme
        public Dictionary<string, int> IngressosPorFilme()
        {
            var ingressos = _ingressoServico.ListarIngressos();
            return ingressos
                .Where(i => i.Sessao?.Filme?.Titulo != null)
                .GroupBy(i => i.Sessao!.Filme!.Titulo)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Sessões com maior ocupação
        public List<(Sessao sessao, int ingressosVendidos, decimal percentualOcupacao)> SessoesComMaiorOcupacao(int top = 5)
        {
            var sessoes = _sessaoServico.ListarSessoes();
            return sessoes
                .Where(s => s.Sala != null)
                .Select(s => (
                    sessao: s,
                    ingressosVendidos: s.Ingressos.Count,
                    percentualOcupacao: s.Sala!.Capacidade > 0 ? (decimal)s.Ingressos.Count / s.Sala.Capacidade * 100 : 0
                ))
                .OrderByDescending(x => x.percentualOcupacao)
                .Take(top)
                .ToList();
        }

        // Receita total de pedidos
        public decimal ReceitaTotalPedidos()
        {
            var pedidos = _pedidoServico.ListarPedidos();
            return pedidos.Sum(p => p.ValorTotal);
        }

        // Produtos mais vendidos
        public Dictionary<string, int> ProdutosMaisVendidos()
        {
            var pedidos = _pedidoServico.ListarPedidos();
            return pedidos
                .SelectMany(p => p.Itens)
                .Where(i => i.Produto != null)
                .GroupBy(i => i.Produto!.Nome)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantidade));
        }

        // Produtos com estoque baixo
        public List<ProdutoAlimento> ProdutosEstoqueBaixo()
        {
            return _produtoServico.ListarProdutosEstoqueBaixo();
        }

        // Receita total geral (ingressos + pedidos)
        public decimal ReceitaTotalGeral()
        {
            return ReceitaTotalIngressos() + ReceitaTotalPedidos();
        }

        // Relatório de vendas por período
        public (int ingressos, decimal receitaIngressos, int pedidos, decimal receitaPedidos) VendasPorPeriodo(DateTime inicio, DateTime fim)
        {
            var ingressos = _ingressoServico.ListarIngressos()
                .Where(i => i.DataCompra >= inicio && i.DataCompra <= fim)
                .ToList();

            var pedidos = _pedidoServico.ListarPedidos()
                .Where(p => p.DataPedido >= inicio && p.DataPedido <= fim)
                .ToList();

            return (
                ingressos: ingressos.Count,
                receitaIngressos: ingressos.Sum(i => i.CalcularPreco(i.Sessao?.PrecoFinal ?? 0)),
                pedidos: pedidos.Count,
                receitaPedidos: pedidos.Sum(p => p.ValorTotal)
            );
        }

        // Taxa de ocupação média das sessões
        public decimal TaxaOcupacaoMedia()
        {
            var sessoes = _sessaoServico.ListarSessoes();
            if (sessoes.Count == 0)
            {
                return 0;
            }

            var ocupacoes = sessoes
                .Where(s => s.Sala != null && s.Sala.Capacidade > 0)
                .Select(s => (decimal)s.Ingressos.Count / s.Sala!.Capacidade * 100);

            return ocupacoes.Any() ? ocupacoes.Average() : 0;
        }

        // Filmes com sessoes no periodo (cartaz)
        public List<(Filme filme, List<Sessao> sessoes)> FilmesEmCartaz(DateTime? inicio = null, DateTime? fim = null, bool? somenteDisponiveis = null)
        {
            var dataInicio = inicio ?? DateTime.Now;
            var dataFim = fim ?? dataInicio.AddDays(DiasCartazPadrao);

            if (dataFim < dataInicio)
            {
                return new List<(Filme filme, List<Sessao> sessoes)>();
            }

            var sessoesNoPeriodo = _sessaoServico.ListarSessoes()
                .Where(s => s.Filme != null && s.DataHorario >= dataInicio && s.DataHorario <= dataFim)
                .ToList();

            if (somenteDisponiveis.HasValue)
            {
                var filtrarDisponiveis = somenteDisponiveis.Value;
                sessoesNoPeriodo = sessoesNoPeriodo
                    .Where(s => s.Sala != null && ((s.Sala.Capacidade - s.Ingressos.Count) > 0) == filtrarDisponiveis)
                    .ToList();
            }

            return sessoesNoPeriodo
                .GroupBy(s => s.Filme!)
                .Select(g => (filme: g.Key, sessoes: g.OrderBy(s => s.DataHorario).ToList()))
                .ToList();
        }

        // Taxa de ocupacao por sala no periodo
        public List<(Sala sala, int ingressosVendidos, int capacidadeTotal, decimal taxaOcupacao)> OcupacaoPorSala(DateTime? inicio = null, DateTime? fim = null)
        {
            var dataInicio = inicio ?? DateTime.Now;
            var dataFim = fim ?? dataInicio.AddDays(DiasCartazPadrao);

            if (dataFim < dataInicio)
            {
                return new List<(Sala sala, int ingressosVendidos, int capacidadeTotal, decimal taxaOcupacao)>();
            }

            var sessoesNoPeriodo = _sessaoServico.ListarSessoes()
                .Where(s => s.Sala != null && s.DataHorario >= dataInicio && s.DataHorario <= dataFim)
                .ToList();

            return sessoesNoPeriodo
                .GroupBy(s => s.Sala!)
                .Select(g =>
                {
                    var ingressos = g.Sum(s => s.Ingressos.Count);
                    var capacidadeTotal = g.Sum(s => s.Sala!.Capacidade);
                    var taxa = capacidadeTotal > 0 ? (decimal)ingressos / capacidadeTotal * 100 : 0m;
                    return (sala: g.Key, ingressosVendidos: ingressos, capacidadeTotal, taxaOcupacao: taxa);
                })
                .OrderByDescending(r => r.taxaOcupacao)
                .ToList();
        }
    }
}
