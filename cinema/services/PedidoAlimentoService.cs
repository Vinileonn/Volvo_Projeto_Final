using cinema.models;
using cinema.models.UsuarioModel;
using cinema.exceptions;

namespace cinema.services
{
    public class PedidoAlimentoService
    {
        private readonly List<PedidoAlimento> pedidos;
        private readonly ProdutoAlimentoService produtoService;

        public PedidoAlimentoService(ProdutoAlimentoService produtoService)
        {
            pedidos = new List<PedidoAlimento>();
            this.produtoService = produtoService;
        }

        // CREATE - criar novo pedido
        public PedidoAlimento CriarPedido(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new DadosInvalidosException("Cliente não pode ser nulo.");
            }

            var pedido = new PedidoAlimento(pedidos.Count > 0 ? pedidos.Max(p => p.Id) + 1 : 1, 0);
            pedidos.Add(pedido);

            if (!cliente.Pedidos.Contains(pedido))
            {
                cliente.Pedidos.Add(pedido);
            }

            return pedido;
        }

        // CREATE - adicionar item ao pedido
        public void AdicionarItem(int pedidoId, int produtoId, int quantidade)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Id == pedidoId);
            var produto = produtoService.ObterProduto(produtoId);

            if (pedido == null || produto == null)
            {   
                throw new RecursoNaoEncontradoException("Pedido ou produto não encontrado.");
            }

            if (quantidade <= 0)
            {
                throw new DadosInvalidosException("Quantidade deve ser maior que zero.");
            }

            if (!produtoService.VerificarDisponibilidade(produtoId, quantidade))
            {
                throw new OperacaoNaoPermitidaException($"Estoque insuficiente. '{produto.Nome}': {produto.EstoqueAtual} disponível.");
            }

            produtoService.ReduzirEstoque(produtoId, quantidade);

            var itemId = pedido.Itens.Count > 0 ? pedido.Itens.Max(i => i.Id) + 1 : 1;
            var item = new ItemPedidoAlimento(itemId, produto, quantidade, produto.Preco);
            pedido.AdicionarItem(item);
        }

        // READ - obter pedido por id
        public PedidoAlimento? ObterPedido(int id)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
            {
                throw new RecursoNaoEncontradoException($"Pedido com ID {id} não encontrado.");
            }
            return pedido;
        }

        // READ - listar todos os pedidos
        public List<PedidoAlimento> ListarPedidos()
        {
            return new List<PedidoAlimento>(pedidos);
        }

        // UPDATE - remover item do pedido e devolver ao estoque
        public void RemoverItem(int pedidoId, int itemId)
        {
            var pedido = ObterPedido(pedidoId);

            if (pedido == null)
            {
                throw new RecursoNaoEncontradoException($"Item com ID {itemId} não encontrado no pedido.");
            }

            var item = pedido.Itens.FirstOrDefault(i => i.Id == itemId);
            if (item == null || item.Produto == null)
            {
                throw new RecursoNaoEncontradoException($"Item com ID {itemId} não encontrado no pedido.");
            }

            produtoService.AdicionarEstoque(item.Produto.Id, item.Quantidade);
            pedido.ValorTotal -= item.Preco * item.Quantidade;
            pedido.Itens.Remove(item);
        }

        // DELETE - cancelar pedido e devolver todos os itens ao estoque
        public void CancelarPedido(int id)
        {
            var pedido = ObterPedido(id);

            if (pedido == null)
            {
                throw new RecursoNaoEncontradoException($"Pedido com ID {id} não encontrado.");
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
                throw new RecursoNaoEncontradoException($"Pedido com ID {pedidoId} não encontrado.");
            }
            
            return pedido.ValorTotal;
        }
    }
}
