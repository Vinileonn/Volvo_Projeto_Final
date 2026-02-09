using cinecore.dados;
using cinecore.modelos;
using cinecore.excecoes;
using cinecore.enums;
using cinecore.utilitarios;
using Microsoft.EntityFrameworkCore;

namespace cinecore.servicos
{
    public class PedidoAlimentoServico
    {
        private readonly CineFlowContext _context;
        private readonly ProdutoAlimentoServico produtoService;
        private const decimal DescontoAniversarioPedidos = 0.10m;
        private const decimal ValorPorPonto = 0.10m;
        private const int PrazoCancelamentoMinutos = 15;

        public PedidoAlimentoServico(CineFlowContext context, ProdutoAlimentoServico produtoService)
        {
            _context = context;
            this.produtoService = produtoService;
        }

        public PedidoAlimento CriarPedido(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new DadosInvalidosExcecao("Cliente não pode ser nulo.");
            }

            var pedido = new PedidoAlimento(0, 0);
            pedido.Cliente = cliente;
            _context.PedidosAlimento.Add(pedido);
            _context.SaveChanges();

            return pedido;
        }

        public void AdicionarItem(int pedidoId, int produtoId, int quantidade, float? precoUnitario = null)
        {
            var pedido = _context.PedidosAlimento
                .Include(p => p.Itens)
                .Include(p => p.Cliente)
                .FirstOrDefault(p => p.Id == pedidoId);
            var produto = produtoService.ObterProduto(produtoId);

            if (pedido == null || produto == null)
            {   
                throw new RecursoNaoEncontradoExcecao("Pedido ou produto não encontrado.");
            }

            if (quantidade <= 0)
            {
                throw new DadosInvalidosExcecao("Quantidade deve ser maior que zero.");
            }

            if (!produtoService.VerificarDisponibilidade(produtoId, quantidade))
            {
                throw new OperacaoNaoPermitidaExcecao($"Estoque insuficiente. '{produto.Nome}': {produto.EstoqueAtual} disponível.");
            }

            produtoService.ReduzirEstoque(produtoId, quantidade);

            var precoFinal = precoUnitario.HasValue && precoUnitario.Value >= 0 ? precoUnitario.Value : produto.Preco;
            var item = new ItemPedidoAlimento(0, quantidade, precoFinal, produto);
            pedido.AdicionarItem(item);

            _context.SaveChanges();
        }

        public PedidoAlimento? ObterPedido(int id)
        {
            var pedido = _context.PedidosAlimento
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(p => p.Cliente)
                .FirstOrDefault(p => p.Id == id);
            if (pedido == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Pedido com ID {id} não encontrado.");
            }
            return pedido;
        }

        public List<PedidoAlimento> ListarPedidos()
        {
            return _context.PedidosAlimento
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(p => p.Cliente)
                .ToList();
        }

        public void RemoverItem(int pedidoId, int itemId)
        {
            var pedido = ObterPedido(pedidoId);

            if (pedido == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Item com ID {itemId} não encontrado no pedido.");
            }

            var item = pedido.Itens.FirstOrDefault(i => i.Id == itemId);
            if (item == null || item.Produto == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Item com ID {itemId} não encontrado no pedido.");
            }

            produtoService.AdicionarEstoque(item.Produto.Id, item.Quantidade);
            pedido.ValorTotal -= item.Preco * item.Quantidade;
            pedido.Itens.Remove(item);
            _context.ItensPedidoAlimento.Remove(item);
            _context.SaveChanges();
        }

        public void CancelarPedido(int id)
        {
            var pedido = ObterPedido(id);

            if (pedido == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Pedido com ID {id} não encontrado.");
            }

            if (pedido.FormaPagamento.HasValue && DateTime.Now > pedido.DataPedido.AddMinutes(PrazoCancelamentoMinutos))
            {
                throw new OperacaoNaoPermitidaExcecao("Cancelamento permitido apenas ate 15 minutos apos o pedido pago.");
            }

            foreach (var item in pedido.Itens)
            {
                if (item.Produto != null)
                {
                    produtoService.AdicionarEstoque(item.Produto.Id, item.Quantidade);
                }
            }

            _context.PedidosAlimento.Remove(pedido);
            _context.SaveChanges();
        }

        // Calcular total do pedido
        public float CalcularTotal(int pedidoId)
        {
            var pedido = ObterPedido(pedidoId);
            if (pedido == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Pedido com ID {pedidoId} não encontrado.");
            }
            
            return pedido.ValorTotal;
        }

        // PAGAMENTO - registra forma de pagamento e calcula troco se for dinheiro.
        public void RegistrarPagamento(int pedidoId, FormaPagamento formaPagamento, decimal valorPago = 0m, int pontosUsados = 0)
        {
            var pedido = ObterPedido(pedidoId);
            if (pedido == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Pedido com ID {pedidoId} nao encontrado.");
            }

            var total = Math.Round((decimal)pedido.ValorTotal, 2);
            if (pedido.Cliente != null && pedido.Cliente.EhMesAniversario(DateTime.Now) && pedido.ValorDesconto <= 0)
            {
                var desconto = Math.Round(total * DescontoAniversarioPedidos, 2);
                if (desconto > 0)
                {
                    pedido.ValorDesconto = (float)desconto;
                    pedido.MotivoDesconto = "Desconto de aniversario";
                    total -= desconto;
                    pedido.ValorTotal = (float)total;
                }
            }

            if (pedido.Cliente != null && pontosUsados > 0)
            {
                if (!pedido.Cliente.TentarUsarPontos(pontosUsados))
                {
                    throw new OperacaoNaoPermitidaExcecao("Pontos insuficientes.");
                }

                var descontoPontos = Math.Round(pontosUsados * ValorPorPonto, 2);
                if (descontoPontos > 0)
                {
                    total -= descontoPontos;
                    if (total < 0)
                    {
                        total = 0;
                    }
                    pedido.ValorTotal = (float)total;
                    pedido.PontosUsados = pontosUsados;
                }
            }

            pedido.FormaPagamento = formaPagamento;
            pedido.TrocoDetalhado = new Dictionary<decimal, int>();
            pedido.ValorTroco = 0m;

            if (formaPagamento == FormaPagamento.Dinheiro)
            {
                if (valorPago < total)
                {
                    throw new DadosInvalidosExcecao("Valor pago menor que o total.");
                }

                pedido.ValorPago = valorPago;
                pedido.TrocoDetalhado = CalculadoraTroco.CalcularTroco(total, valorPago, out var troco);
                pedido.ValorTroco = troco;
                _context.SaveChanges();
                return;
            }

            // Para outras formas, considera pagamento exato.
            pedido.ValorPago = total;
            if (pedido.Cliente != null)
            {
                var pontosGerados = (int)Math.Floor((double)total);
                pedido.PontosGerados = pontosGerados;
                pedido.Cliente.AdicionarPontos(pontosGerados);
            }

            _context.SaveChanges();
        }
    }
}
