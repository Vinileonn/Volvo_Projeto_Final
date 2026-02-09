using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.servicos;
using cinecore.DTOs.Relatorio;
using cinecore.DTOs.Filme;

namespace cinecore.controladores
{
    /// <summary>
    /// Controller para gerenciar relatórios e estatísticas na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RelatorioControlador : ControllerBase
    {
        private readonly RelatorioServico _relatorioServico;
        private readonly IMapper _mapper;

        public RelatorioControlador(RelatorioServico relatorioServico, IMapper mapper)
        {
            _relatorioServico = relatorioServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém o total de ingressos vendidos
        /// </summary>
        [HttpGet("ingressos/total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> TotalIngressosVendidos()
        {
            try
            {
                var total = _relatorioServico.TotalIngressosVendidos();
                return Ok(new { total, mensagem = $"Total de ingressos vendidos: {total}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao calcular total de ingressos.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a receita total de ingressos
        /// </summary>
        [HttpGet("ingressos/receita")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> ReceitaTotalIngressos()
        {
            try
            {
                var total = _relatorioServico.ReceitaTotalIngressos();
                return Ok(new { total, mensagem = $"Receita total de ingressos: R$ {total:F2}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao calcular receita de ingressos.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a quantidade de ingressos vendidos por filme
        /// </summary>
        [HttpGet("ingressos/por-filme")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> IngressosPorFilme()
        {
            try
            {
                var dados = _relatorioServico.IngressosPorFilme();
                if (dados.Count == 0)
                {
                    return Ok(new { dados, mensagem = "Nenhum ingresso vendido ainda." });
                }
                return Ok(new { dados, mensagem = "Relatório de ingressos por filme gerado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao gerar relatório de ingressos por filme.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém as sessões com maior ocupação
        /// </summary>
        [HttpGet("sessoes/maior-ocupacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> SessoesComMaiorOcupacao([FromQuery] int top = 5)
        {
            try
            {
                var dados = _relatorioServico.SessoesComMaiorOcupacao(top);
                if (dados.Count == 0)
                {
                    return Ok(new { dados, mensagem = "Nenhuma sessão encontrada." });
                }
                
                // Transformar os dados para um formato mais amigável para JSON
                var resultado = dados.Select(d => new
                {
                    sessaoId = d.sessao.Id,
                    filmeNome = d.sessao.Filme?.Titulo ?? "Sem título",
                    salaNome = d.sessao.Sala?.Nome ?? "Sem sala",
                    dataHorario = d.sessao.DataHorario,
                    ingressosVendidos = d.ingressosVendidos,
                    capacidadeSala = d.sessao.Sala?.Capacidade ?? 0,
                    percentualOcupacao = d.percentualOcupacao
                }).ToList();

                return Ok(new { dados = resultado, mensagem = "Relatório de sessões com maior ocupação gerado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao gerar relatório de ocupação.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a receita total de pedidos de alimentos
        /// </summary>
        [HttpGet("pedidos/receita")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> ReceitaTotalPedidos()
        {
            try
            {
                var total = _relatorioServico.ReceitaTotalPedidos();
                return Ok(new { total, mensagem = $"Receita total de pedidos: R$ {total:F2}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao calcular receita de pedidos.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os produtos mais vendidos
        /// </summary>
        [HttpGet("produtos/mais-vendidos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> ProdutosMaisVendidos()
        {
            try
            {
                var dados = _relatorioServico.ProdutosMaisVendidos();
                if (dados.Count == 0)
                {
                    return Ok(new { dados, mensagem = "Nenhum produto vendido ainda." });
                }
                return Ok(new { dados, mensagem = "Relatório de produtos mais vendidos gerado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao gerar relatório de produtos mais vendidos.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os produtos com estoque baixo
        /// </summary>
        [HttpGet("produtos/estoque-baixo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> ProdutosEstoqueBaixo()
        {
            try
            {
                var produtos = _relatorioServico.ProdutosEstoqueBaixo();
                if (produtos.Count == 0)
                {
                    return Ok(new { produtos, mensagem = "Nenhum produto com estoque baixo." });
                }
                return Ok(new { produtos, mensagem = "Relatório de produtos com estoque baixo gerado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao gerar relatório de estoque baixo.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a receita total geral (ingressos + pedidos)
        /// </summary>
        [HttpGet("receita-total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> ReceitaTotalGeral()
        {
            try
            {
                var total = _relatorioServico.ReceitaTotalGeral();
                return Ok(new { total, mensagem = $"Receita total geral: R$ {total:F2}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao calcular receita total geral.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém relatório de vendas em um período específico
        /// </summary>
        [HttpGet("vendas-por-periodo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<object> VendasPorPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            try
            {
                if (inicio > fim)
                {
                    return BadRequest(new { mensagem = "Data inicial não pode ser maior que a data final." });
                }

                var resultado = _relatorioServico.VendasPorPeriodo(inicio, fim);
                return Ok(new
                {
                    periodo = new { inicio, fim },
                    ingressos = resultado.ingressos,
                    receitaIngressos = resultado.receitaIngressos,
                    pedidos = resultado.pedidos,
                    receitaPedidos = resultado.receitaPedidos,
                    receitaTotal = resultado.receitaIngressos + resultado.receitaPedidos,
                    mensagem = "Relatório de vendas por período gerado."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao gerar relatório por período.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a taxa média de ocupação das sessões
        /// </summary>
        [HttpGet("sessoes/taxa-ocupacao-media")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> TaxaOcupacaoMedia()
        {
            try
            {
                var taxa = _relatorioServico.TaxaOcupacaoMedia();
                return Ok(new { taxa, mensagem = $"Taxa média de ocupação: {taxa:F2}%." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao calcular taxa de ocupação média.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Lista filmes com sessoes disponiveis no periodo (cartaz)
        /// </summary>
        [HttpGet("cartaz")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> FilmesEmCartaz([FromQuery] DateTime? inicio = null, [FromQuery] DateTime? fim = null, [FromQuery] bool? disponiveis = null)
        {
            try
            {
                var dados = _relatorioServico.FilmesEmCartaz(inicio, fim, disponiveis);
                var resultado = dados.Select(item => new CartazFilmeDto
                {
                    Filme = _mapper.Map<FilmeDto>(item.filme),
                    Sessoes = item.sessoes.Select(s => new CartazSessaoDto
                    {
                        Id = s.Id,
                        DataHorario = s.DataHorario,
                        PrecoFinal = s.PrecoFinal,
                        Tipo = s.Tipo,
                        Idioma = s.Idioma,
                        SalaId = s.Sala?.Id ?? 0,
                        SalaNome = s.Sala?.Nome ?? ""
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    inicio = inicio ?? DateTime.Now,
                    fim = fim ?? (inicio ?? DateTime.Now).AddDays(7),
                    disponiveis,
                    quantidade = resultado.Count,
                    filmes = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao gerar cartaz.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Lista salas e taxa de ocupacao no periodo
        /// </summary>
        [HttpGet("salas/ocupacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> OcupacaoPorSala([FromQuery] DateTime? inicio = null, [FromQuery] DateTime? fim = null)
        {
            try
            {
                var dados = _relatorioServico.OcupacaoPorSala(inicio, fim)
                    .Select(r => new OcupacaoSalaDto
                    {
                        SalaId = r.sala.Id,
                        SalaNome = r.sala.Nome,
                        CapacidadeTotal = r.capacidadeTotal,
                        IngressosVendidos = r.ingressosVendidos,
                        TaxaOcupacao = r.taxaOcupacao
                    })
                    .ToList();

                return Ok(new
                {
                    inicio = inicio ?? DateTime.Now,
                    fim = fim ?? (inicio ?? DateTime.Now).AddDays(7),
                    quantidade = dados.Count,
                    salas = dados
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao gerar ocupacao por sala.", erro = ex.Message });
            }
        }
    }
}
