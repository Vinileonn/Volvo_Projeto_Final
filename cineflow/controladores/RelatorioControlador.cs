using cineflow.modelos;
using cineflow.servicos;

namespace cineflow.controladores
{
    // Renomeado de RelatorioController para RelatorioControlador.
    public class RelatorioControlador
    {
        private readonly RelatorioServico RelatorioServico;

        public RelatorioControlador(RelatorioServico RelatorioServico)
        {
            this.RelatorioServico = RelatorioServico;
        }

        // RELATORIO - total de ingressos vendidos
        public (int total, string mensagem) TotalIngressosVendidos()
        {
            try
            {
                var total = RelatorioServico.TotalIngressosVendidos();
                return (total, $"Total de ingressos vendidos: {total}.");
            }
            catch (Exception)
            {
                return (0, "Erro inesperado ao calcular total de ingressos.");
            }
        }

        // RELATORIO - receita total de ingressos
        public (float total, string mensagem) ReceitaTotalIngressos()
        {
            try
            {
                var total = RelatorioServico.ReceitaTotalIngressos();
                return (total, $"Receita total de ingressos: R$ {total:F2}.");
            }
            catch (Exception)
            {
                return (0, "Erro inesperado ao calcular receita de ingressos.");
            }
        }

        // RELATORIO - ingressos vendidos por filme
        public (Dictionary<string, int> dados, string mensagem) IngressosPorFilme()
        {
            try
            {
                var dados = RelatorioServico.IngressosPorFilme();
                if (dados.Count == 0)
                {
                    return (dados, "Nenhum ingresso vendido ainda.");
                }
                return (dados, "Relatório de ingressos por filme gerado.");
            }
            catch (Exception)
            {
                return (new Dictionary<string, int>(), "Erro inesperado ao gerar relatório de ingressos por filme.");
            }
        }

        // RELATORIO - sessões com maior ocupação
        public (List<(Sessao sessao, int ingressosVendidos, float percentualOcupacao)> dados, string mensagem) SessoesComMaiorOcupacao(int top = 5)
        {
            try
            {
                var dados = RelatorioServico.SessoesComMaiorOcupacao(top);
                if (dados.Count == 0)
                {
                    return (dados, "Nenhuma sessão encontrada.");
                }
                return (dados, "Relatório de sessões com maior ocupação gerado.");
            }
            catch (Exception)
            {
                return (new List<(Sessao sessao, int ingressosVendidos, float percentualOcupacao)>(), "Erro inesperado ao gerar relatório de ocupação.");
            }
        }

        // RELATORIO - receita total de pedidos
        public (float total, string mensagem) ReceitaTotalPedidos()
        {
            try
            {
                var total = RelatorioServico.ReceitaTotalPedidos();
                return (total, $"Receita total de pedidos: R$ {total:F2}.");
            }
            catch (Exception)
            {
                return (0, "Erro inesperado ao calcular receita de pedidos.");
            }
        }

        // RELATORIO - produtos mais vendidos
        public (Dictionary<string, int> dados, string mensagem) ProdutosMaisVendidos()
        {
            try
            {
                var dados = RelatorioServico.ProdutosMaisVendidos();
                if (dados.Count == 0)
                {
                    return (dados, "Nenhum produto vendido ainda.");
                }
                return (dados, "Relatório de produtos mais vendidos gerado.");
            }
            catch (Exception)
            {
                return (new Dictionary<string, int>(), "Erro inesperado ao gerar relatório de produtos mais vendidos.");
            }
        }

        // RELATORIO - produtos com estoque baixo
        public (List<ProdutoAlimento> produtos, string mensagem) ProdutosEstoqueBaixo()
        {
            try
            {
                var produtos = RelatorioServico.ProdutosEstoqueBaixo();
                if (produtos.Count == 0)
                {
                    return (produtos, "Nenhum produto com estoque baixo.");
                }
                return (produtos, "Relatório de produtos com estoque baixo gerado.");
            }
            catch (Exception)
            {
                return (new List<ProdutoAlimento>(), "Erro inesperado ao gerar relatório de estoque baixo.");
            }
        }

        // RELATORIO - receita total geral
        public (float total, string mensagem) ReceitaTotalGeral()
        {
            try
            {
                var total = RelatorioServico.ReceitaTotalGeral();
                return (total, $"Receita total geral: R$ {total:F2}.");
            }
            catch (Exception)
            {
                return (0, "Erro inesperado ao calcular receita total geral.");
            }
        }

        // RELATORIO - vendas por período
        public (int ingressos, float receitaIngressos, int pedidos, float receitaPedidos, string mensagem) VendasPorPeriodo(DateTime inicio, DateTime fim)
        {
            try
            {
                if (inicio > fim)
                {
                    return (0, 0, 0, 0, "Data inicial não pode ser maior que a data final.");
                }

                var resultado = RelatorioServico.VendasPorPeriodo(inicio, fim);
                return (resultado.ingressos, resultado.receitaIngressos, resultado.pedidos, resultado.receitaPedidos,
                    "Relatório de vendas por período gerado.");
            }
            catch (Exception)
            {
                return (0, 0, 0, 0, "Erro inesperado ao gerar relatório por período.");
            }
        }

        // RELATORIO - taxa de ocupação média das sessões
        public (float taxa, string mensagem) TaxaOcupacaoMedia()
        {
            try
            {
                var taxa = RelatorioServico.TaxaOcupacaoMedia();
                return (taxa, $"Taxa média de ocupação: {taxa:F2}%.");
            }
            catch (Exception)
            {
                return (0, "Erro inesperado ao calcular taxa de ocupação média.");
            }
        }
    }
}





