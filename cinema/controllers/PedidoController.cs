using cinema.models;
using cinema.models.UsuarioModel;
using cinema.services;
using cinema.exceptions;

namespace cinema.controllers
{
    public class PedidoController
    {
        private readonly PedidoAlimentoService pedidoService;

        public PedidoController(PedidoAlimentoService pedidoService)
        {
            this.pedidoService = pedidoService;
        }

        // CREATE - criar novo pedido
        public (PedidoAlimento? pedido, string mensagem) CriarPedido(Cliente cliente)
        {
            try
            {
                var pedido = pedidoService.CriarPedido(cliente);
                return (pedido, "Pedido criado com sucesso.");
            }
            catch (DadosInvalidosException ex)
            {
                return (null, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao criar pedido.");
            }
        }

        // CREATE - adicionar item ao pedido
        public (bool sucesso, string mensagem) AdicionarItem(int pedidoId, int produtoId, int quantidade)
        {
            try
            {
                pedidoService.AdicionarItem(pedidoId, produtoId, quantidade);
                return (true, "Item adicionado ao pedido com sucesso.");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (ErroOperacaoCriticaException ex)
            {
                return (false, $"Erro crítico na operação: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao adicionar item.");
            }
        }

        // READ - obter pedido por id
        public (PedidoAlimento? pedido, string mensagem) ObterPedido(int id)
        {
            try
            {
                var pedido = pedidoService.ObterPedido(id);
                return (pedido, "Pedido obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter pedido.");
            }
        }

        // READ - listar todos os pedidos
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

        // UPDATE - remover item do pedido
        public (bool sucesso, string mensagem) RemoverItem(int pedidoId, int itemId)
        {
            try
            {
                pedidoService.RemoverItem(pedidoId, itemId);
                return (true, "Item removido do pedido com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao remover item.");
            }
        }

        // DELETE - cancelar pedido
        public (bool sucesso, string mensagem) CancelarPedido(int id)
        {
            try
            {
                pedidoService.CancelarPedido(id);
                return (true, "Pedido cancelado com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
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
            catch (RecursoNaoEncontradoException ex)
            {
                return (0, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (0, "Erro inesperado ao calcular total.");
            }
        }
    }
}
