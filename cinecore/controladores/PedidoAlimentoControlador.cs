using Microsoft.AspNetCore.Mvc;
using cinecore.modelos;
using cinecore.servicos;
using cinecore.enums;
using cinecore.excecoes;

namespace cinecore.controladores
{
    /// <summary>
    /// Controller para gerenciar operações de Pedidos de Alimento na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoAlimentoControlador : ControllerBase
    {
        private readonly PedidoAlimentoServico _pedidoServico;
        private readonly UsuarioServico _usuarioServico;

        public PedidoAlimentoControlador(PedidoAlimentoServico pedidoServico, UsuarioServico usuarioServico)
        {
            _pedidoServico = pedidoServico;
            _usuarioServico = usuarioServico;
        }

        /// <summary>
        /// Cria um novo pedido de alimento
        /// </summary>
        [HttpPost("Criar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PedidoAlimento> CriarPedido([FromBody] CriarPedidoRequest request)
        {
            try
            {
                var cliente = _usuarioServico.ObterUsuario(request.ClienteId) as Cliente;
                
                if (cliente == null)
                {
                    return BadRequest(new { mensagem = "Usuário informado não é um cliente." });
                }

                var pedido = _pedidoServico.CriarPedido(cliente);
                return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, pedido);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Adiciona um item ao pedido
        /// </summary>
        [HttpPost("{pedidoId}/AdicionarItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult AdicionarItem(int pedidoId, [FromBody] AdicionarItemRequest request)
        {
            try
            {
                _pedidoServico.AdicionarItem(pedidoId, request.ProdutoId, request.Quantidade, request.PrecoUnitario);
                return Ok(new { mensagem = "Item adicionado ao pedido com sucesso." });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (ErroOperacaoCriticaExcecao ex)
            {
                return StatusCode(500, new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um pedido por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PedidoAlimento> ObterPedido(int id)
        {
            try
            {
                var pedido = _pedidoServico.ObterPedido(id);
                return Ok(pedido);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os pedidos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<PedidoAlimento>> ListarPedidos()
        {
            var pedidos = _pedidoServico.ListarPedidos();
            return Ok(pedidos);
        }

        /// <summary>
        /// Remove um item do pedido
        /// </summary>
        [HttpDelete("{pedidoId}/RemoverItem/{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult RemoverItem(int pedidoId, int itemId)
        {
            try
            {
                _pedidoServico.RemoverItem(pedidoId, itemId);
                return Ok(new { mensagem = "Item removido do pedido com sucesso." });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Cancela um pedido
        /// </summary>
        [HttpPost("Cancelar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult CancelarPedido(int id)
        {
            try
            {
                _pedidoServico.CancelarPedido(id);
                return Ok(new { mensagem = "Pedido cancelado com sucesso." });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Calcula o total do pedido
        /// </summary>
        [HttpGet("{id}/CalcularTotal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<decimal> CalcularTotal(int id)
        {
            try
            {
                var total = _pedidoServico.CalcularTotal(id);
                return Ok(new { pedidoId = id, total = total });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Registra o pagamento do pedido
        /// </summary>
        [HttpPost("{id}/RegistrarPagamento")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult RegistrarPagamento(int id, [FromBody] RegistrarPagamentoRequest request)
        {
            try
            {
                _pedidoServico.RegistrarPagamento(id, request.FormaPagamento, request.ValorPago, request.PontosUsados);
                return Ok(new { mensagem = "Pagamento registrado com sucesso." });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }

    public class CriarPedidoRequest
    {
        public int ClienteId { get; set; }
    }

    public class AdicionarItemRequest
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal? PrecoUnitario { get; set; }
    }

    public class RegistrarPagamentoRequest
    {
        public FormaPagamento FormaPagamento { get; set; }
        public decimal ValorPago { get; set; } = 0m;
        public int PontosUsados { get; set; } = 0;
    }
}
