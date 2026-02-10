using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.Models;
using cinecore.Services;
using cinecore.DTOs.ProdutoAlimento;
using cinecore.Exceptions;

namespace cinecore.Controllers
{
    /// <summary>
    /// Controller para gerenciar operações de Produtos Alimentícios na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoAlimentoControlador : ControllerBase
    {
        private readonly ProdutoAlimentoServico _produtoServico;
        private readonly IMapper _mapper;

        public ProdutoAlimentoControlador(ProdutoAlimentoServico produtoServico, IMapper mapper)
        {
            _produtoServico = produtoServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria um novo produto alimentício
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPost("Criar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ProdutoAlimentoDto> CriarProduto([FromBody] CriarProdutoAlimentoDto criarProdutoDto)
        {
            try
            {
                var produto = _mapper.Map<ProdutoAlimento>(criarProdutoDto);
                _produtoServico.CriarProduto(produto);
                var produtoDto = _mapper.Map<ProdutoAlimentoDto>(produto);
                
                return CreatedAtAction(nameof(ObterProduto), new { id = produtoDto.Id }, produtoDto);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um produto alimentício por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProdutoAlimentoDto> ObterProduto(int id)
        {
            var produto = _produtoServico.ObterProduto(id);
            if (produto == null)
            {
                return NotFound(new { mensagem = $"Produto com ID {id} não encontrado." });
            }

            var produtoDto = _mapper.Map<ProdutoAlimentoDto>(produto);
            return Ok(produtoDto);
        }

        /// <summary>
        /// Obtém uma cortesia de pré-estreia disponível
        /// </summary>
        [HttpGet("CortesiaPreEstreia")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProdutoAlimentoDto> ObterCortesiaPreEstreia()
        {
            var produto = _produtoServico.ObterCortesiaPreEstreiaDisponivel();
            if (produto == null)
            {
                return NotFound(new { mensagem = "Nenhuma cortesia de pré-estreia disponível." });
            }

            var produtoDto = _mapper.Map<ProdutoAlimentoDto>(produto);
            return Ok(produtoDto);
        }

        /// <summary>
        /// Lista todos os produtos alimentícios
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProdutoAlimentoDto>> ListarProdutos()
        {
            var produtos = _produtoServico.ListarProdutos();
            var produtosDto = _mapper.Map<List<ProdutoAlimentoDto>>(produtos);
            return Ok(produtosDto);
        }

        /// <summary>
        /// Busca produtos por nome
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProdutoAlimentoDto>> BuscarPorNome([FromQuery] string nome)
        {
            var produtos = _produtoServico.BuscarPorNome(nome);
            var produtosDto = _mapper.Map<List<ProdutoAlimentoDto>>(produtos);
            return Ok(produtosDto);
        }

        /// <summary>
        /// Lista produtos com estoque baixo
        /// </summary>
        [HttpGet("EstoqueBaixo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProdutoAlimentoDto>> ListarProdutosEstoqueBaixo()
        {
            var produtos = _produtoServico.ListarProdutosEstoqueBaixo();
            var produtosDto = _mapper.Map<List<ProdutoAlimentoDto>>(produtos);
            return Ok(produtosDto);
        }

        /// <summary>
        /// Lista alertas de estoque
        /// </summary>
        [HttpGet("AlertasEstoque")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<string>> ListarAlertasEstoque()
        {
            var alertas = _produtoServico.ListarAlertasEstoque();
            return Ok(alertas);
        }

        /// <summary>
        /// Atualiza um produto alimentício
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProdutoAlimentoDto> AtualizarProduto(int id, [FromBody] AtualizarProdutoAlimentoDto atualizarDto)
        {
            try
            {
                _produtoServico.AtualizarProduto(
                    id,
                    atualizarDto.Nome,
                    atualizarDto.Descricao,
                    atualizarDto.Preco,
                    atualizarDto.EstoqueMinimo,
                    atualizarDto.EhTematico,
                    atualizarDto.TemaFilme,
                    atualizarDto.EhCortesia,
                    atualizarDto.ExclusivoPreEstreia,
                    atualizarDto.Categoria
                );

                var produto = _produtoServico.ObterProduto(id);
                var produtoDto = _mapper.Map<ProdutoAlimentoDto>(produto);
                return Ok(produtoDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um produto alimentício
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletarProduto(int id)
        {
            try
            {
                _produtoServico.DeletarProduto(id);
                return NoContent();
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Adiciona quantidade ao estoque de um produto
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPost("{id}/AdicionarEstoque")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProdutoAlimentoDto> AdicionarEstoque(int id, [FromBody] AtualizarEstoqueDto estoqueDto)
        {
            try
            {
                _produtoServico.AdicionarEstoque(id, estoqueDto.Quantidade);
                var produto = _produtoServico.ObterProduto(id);
                var produtoDto = _mapper.Map<ProdutoAlimentoDto>(produto);
                return Ok(produtoDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Reduz quantidade do estoque de um produto
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPost("{id}/ReduzirEstoque")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProdutoAlimentoDto> ReduzirEstoque(int id, [FromBody] AtualizarEstoqueDto estoqueDto)
        {
            try
            {
                _produtoServico.ReduzirEstoque(id, estoqueDto.Quantidade);
                var produto = _produtoServico.ObterProduto(id);
                var produtoDto = _mapper.Map<ProdutoAlimentoDto>(produto);
                return Ok(produtoDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Verifica disponibilidade de quantidade em estoque
        /// </summary>
        [HttpGet("{id}/VerificarDisponibilidade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> VerificarDisponibilidade(int id, [FromQuery] int quantidade)
        {
            if (quantidade <= 0)
            {
                return BadRequest(new { mensagem = "Quantidade deve ser maior que zero." });
            }

            var disponivel = _produtoServico.VerificarDisponibilidade(id, quantidade);
            return Ok(new { disponivel, quantidade });
        }
    }
}
