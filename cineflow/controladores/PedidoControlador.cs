using cineflow.modelos;
using cineflow.modelos.UsuarioModelo;
using cineflow.servicos;
using cineflow.enumeracoes;
using cineflow.excecoes;

namespace cineflow.controladores
{
    public class PedidoControlador
    {
        private readonly PedidoAlimentoServico pedidoService;

        public PedidoControlador(PedidoAlimentoServico pedidoService)
        {
            this.pedidoService = pedidoService;
        }
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
        public (bool sucesso, string mensagem) AdicionarItem(int pedidoId, int produtoId, int quantidade, float? precoUnitario = null)
        {
            try
            {
                pedidoService.AdicionarItem(pedidoId, produtoId, quantidade, precoUnitario);
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
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operacao nao permitida: {ex.Message}");
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
        public (bool sucesso, string mensagem) RegistrarPagamento(int pedidoId, FormaPagamento formaPagamento, decimal valorPago = 0m, int pontosUsados = 0)
        {
            try
            {
                pedidoService.RegistrarPagamento(pedidoId, formaPagamento, valorPago, pontosUsados);
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
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operacao nao permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao registrar pagamento.");
            }
        }
    }
}





