using cinema.modelos;
using cinema.modelos.UsuarioModelo;
using cinema.servicos;
using cinema.enumeracoes;
using cinema.excecoes;

namespace cinema.controladores
{
    // Renomeado de PedidoController para PedidoControlador.
    public class PedidoControlador
    {
        private readonly PedidoAlimentoServico pedidoService;

        public PedidoControlador(PedidoAlimentoServico pedidoService)
        {
            this.pedidoService = pedidoService;
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        public (PedidoAlimento? pedido, string mensagem) CriarPedido(Cliente cliente)
        {
            try
            {
                var pedido = pedidoService.CriarPedido(cliente);
                return (pedido, "Pedido criado com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (null, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao criar pedido.");
            }
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        public (bool sucesso, string mensagem) AdicionarItem(int pedidoId, int produtoId, int quantidade)
        {
            try
            {
                pedidoService.AdicionarItem(pedidoId, produtoId, quantidade);
                return (true, "Item adicionado ao pedido com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (ErroOperacaoCriticaExcecao ex)
            {
                return (false, $"Erro crítico na operação: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao adicionar item.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (PedidoAlimento? pedido, string mensagem) ObterPedido(int id)
        {
            try
            {
                var pedido = pedidoService.ObterPedido(id);
                return (pedido, "Pedido obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter pedido.");
            }
        }
// ANTIGO: // LER - 
// LER - 
        public (List<PedidoAlimento> pedidos, string mensagem) ListarPedidos()
        {
            try
            {
                var pedidos = pedidoService.ListarPedidos();
                if (pedidos.Count == 0)
                {
                    return (pedidos, "Nenhum pedido encontrado.");
                }
                return (pedidos, $"{pedidos.Count} pedido(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<PedidoAlimento>(), "Erro inesperado ao listar pedidos.");
            }
        }
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
        public (bool sucesso, string mensagem) RemoverItem(int pedidoId, int itemId)
        {
            try
            {
                pedidoService.RemoverItem(pedidoId, itemId);
                return (true, "Item removido do pedido com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao remover item.");
            }
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
        public (bool sucesso, string mensagem) CancelarPedido(int id)
        {
            try
            {
                pedidoService.CancelarPedido(id);
                return (true, "Pedido cancelado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao cancelar pedido.");
            }
        }

        // CALCULO - total do pedido
        public (float total, string mensagem) CalcularTotal(int pedidoId)
        {
            try
            {
                var total = pedidoService.CalcularTotal(pedidoId);
                return (total, $"Total calculado com sucesso: R$ {total:F2}");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (0, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (0, "Erro inesperado ao calcular total.");
            }
        }

        // PAGAMENTO - registrar forma de pagamento do pedido
        public (bool sucesso, string mensagem) RegistrarPagamento(int pedidoId, FormaPagamento formaPagamento, decimal valorPago = 0m)
        {
            try
            {
                pedidoService.RegistrarPagamento(pedidoId, formaPagamento, valorPago);
                return (true, "Pagamento registrado com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados invalidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao registrar pagamento.");
            }
        }
    }
}





