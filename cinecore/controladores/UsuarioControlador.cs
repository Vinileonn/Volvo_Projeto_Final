using Microsoft.AspNetCore.Mvc;
using cinecore.modelos;
using cinecore.servicos;
using cinecore.excecoes;

namespace cinecore.controladores
{
    /// <summary>
    /// Controller para gerenciar operações de Usuários na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioControlador : ControllerBase
    {
        private readonly UsuarioServico _usuarioServico;

        public UsuarioControlador(UsuarioServico usuarioServico)
        {
            _usuarioServico = usuarioServico;
        }

        /// <summary>
        /// Adiciona um novo usuário
        /// </summary>
        [HttpPost("Adicionar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Usuario> AdicionarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                _usuarioServico.AdicionarUsuario(usuario);
                return CreatedAtAction(nameof(ObterUsuario), new { id = usuario.Id }, usuario);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um usuário por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Usuario> ObterUsuario(int id)
        {
            try
            {
                var usuario = _usuarioServico.ObterUsuario(id);
                return Ok(usuario);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um usuário por email
        /// </summary>
        [HttpGet("ObterPorEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Usuario> ObterUsuarioPorEmail([FromQuery] string email)
        {
            try
            {
                var usuario = _usuarioServico.ObterUsuarioPorEmail(email);
                return Ok(usuario);
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
        /// Lista todos os usuários
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Usuario>> ListarUsuarios()
        {
            var usuarios = _usuarioServico.ListarUsuarios();
            return Ok(usuarios);
        }

        /// <summary>
        /// Lista apenas clientes
        /// </summary>
        [HttpGet("Clientes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Cliente>> ListarClientes()
        {
            var clientes = _usuarioServico.ListarClientes();
            return Ok(clientes);
        }

        /// <summary>
        /// Atualiza os dados de um usuário
        /// </summary>
        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult AtualizarUsuario(int id, [FromBody] AtualizarUsuarioRequest request)
        {
            try
            {
                _usuarioServico.AtualizarUsuario(id, request.Nome, request.Email, request.Telefone, request.Endereco);
                return Ok(new { mensagem = "Usuário atualizado com sucesso." });
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
        /// Deleta um usuário
        /// </summary>
        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeletarUsuario(int id)
        {
            try
            {
                _usuarioServico.DeletarUsuario(id);
                return Ok(new { mensagem = "Usuário deletado com sucesso." });
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

    public class AtualizarUsuarioRequest
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Endereco { get; set; }
    }
}
