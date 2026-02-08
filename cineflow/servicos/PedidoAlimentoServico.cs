using cineflow.modelos;
using cineflow.modelos.UsuarioModelo;
using cineflow.excecoes;
using cineflow.enumeracoes;
using cineflow.utilitarios;

namespace cineflow.servicos
{
    public class PedidoAlimentoServico
    {
        private readonly List<PedidoAlimento> pedidos;
        private readonly ProdutoAlimentoServico produtoService;

        public PedidoAlimentoServico(ProdutoAlimentoServico produtoService)
        {
            pedidos = new List<PedidoAlimento>();
            this.produtoService = produtoService;
        }

        public PedidoAlimento CriarPedido(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new DadosInvalidosExcecao("Cliente não pode ser nulo.");
            }

            var pedido = new PedidoAlimento(pedidos.Count > 0 ? pedidos.Max(p => p.Id) + 1 : 1, 0);
            pedidos.Add(pedido);

            if (!cliente.Pedidos.Contains(pedido))
            {
                cliente.Pedidos.Add(pedido);
            }

            return pedido;
        }

        public void AdicionarItem(int pedidoId, int produtoId, int quantidade)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Id == pedidoId);
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

            var itemId = pedido.Itens.Count > 0 ? pedido.Itens.Max(i => i.Id) + 1 : 1;
            var item = new ItemPedidoAlimento(itemId, produto, quantidade, produto.Preco);
            pedido.AdicionarItem(item);
        }

        public PedidoAlimento? ObterPedido(int id)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Pedido com ID {id} não encontrado.");
            }
            return pedido;
        }

        public List<PedidoAlimento> ListarPedidos()
        {
            return new List<PedidoAlimento>(pedidos);
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
        }

        public void CancelarPedido(int id)
        {
            var pedido = ObterPedido(id);

            if (pedido == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Pedido com ID {id} não encontrado.");
            }

            foreach (var item in pedido.Itens)
            {
                if (item.Produto != null)
                {
                    produtoService.AdicionarEstoque(item.Produto.Id, item.Quantidade);
                }
            }

            pedidos.Remove(pedido);
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
        public void RegistrarPagamento(int pedidoId, FormaPagamento formaPagamento, decimal valorPago = 0m)
        {
            var pedido = ObterPedido(pedidoId);
            if (pedido == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Pedido com ID {pedidoId} nao encontrado.");
            }

            var total = Math.Round((decimal)pedido.ValorTotal, 2);

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
                return;
            }

            // Para outras formas, considera pagamento exato.
            pedido.ValorPago = total;
        }
    }
}





