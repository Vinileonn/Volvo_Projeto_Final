using Microsoft.AspNetCore.Mvc;
using cinecore.Models;
using cinecore.Services;
using cinecore.Exceptions;
using cinecore.DTOs.Usuario;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace cinecore.Controllers
{
    /// <summary>
    /// Controller para gerenciar operações de Usuários na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioControlador : ControllerBase
    {
        private readonly UsuarioServico _usuarioServico;
        private readonly IMapper _mapper;

        public UsuarioControlador(UsuarioServico usuarioServico, IMapper mapper)
        {
            _usuarioServico = usuarioServico;
            _mapper = mapper;
        }

        /// <summary>
        /// NOTA: Este controlador também permite criar um novo usuário (Cliente).
        /// </summary>

        /// <summary>
        /// Cadastra um novo cliente
        /// </summary>
        [HttpPost("Registrar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ClienteDto> RegistrarCliente([FromBody] CriarUsuarioDto criarUsuarioDto)
        {
            try
            {
                var cliente = _mapper.Map<Cliente>(criarUsuarioDto);
                _usuarioServico.RegistrarCliente(cliente);

                var clienteDto = _mapper.Map<ClienteDto>(cliente);
                return CreatedAtAction(nameof(ObterUsuario), new { id = clienteDto.Id }, clienteDto);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { sucesso = false, mensagem = $"Dados inválidos: {ex.Message}" });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { sucesso = false, mensagem = $"Operação não permitida: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = "Erro inesperado ao cadastrar cliente.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Cadastra um novo administrador
        /// </summary>

        [HttpPost("RegistrarAdministrador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<AdministradorDto> RegistrarAdministrador([FromBody] CriarAdministradorDto criarAdministradorDto)
        {
            try
            {
                var administrador = _mapper.Map<Administrador>(criarAdministradorDto);
                _usuarioServico.RegistrarAdministrador(administrador);

                var administradorDto = _mapper.Map<AdministradorDto>(administrador);
                return CreatedAtAction(nameof(ObterUsuario), new { id = administradorDto.Id }, administradorDto);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { sucesso = false, mensagem = $"Dados inválidos: {ex.Message}" });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { sucesso = false, mensagem = $"Operação não permitida: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = "Erro inesperado ao cadastrar administrador.", erro = ex.Message });
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
        public ActionResult AtualizarUsuario(int id, [FromBody] AtualizarUsuarioDto request)
        {
            try
            {
                _usuarioServico.AtualizarUsuario(id, request.Nome, request.Email, request.Telefone, request.Endereco);
                var usuario = _usuarioServico.ObterUsuario(id);
                return Ok(usuario);
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
                var usuario = _usuarioServico.ObterUsuario(id);
                _usuarioServico.DeletarUsuario(id);
                return Ok(usuario);
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
}
