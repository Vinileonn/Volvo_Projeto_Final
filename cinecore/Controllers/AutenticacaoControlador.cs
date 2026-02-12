using cinecore.Services;
using cinecore.Exceptions;
using cinecore.DTOs.Usuario;
using Microsoft.AspNetCore.Mvc;

namespace cinecore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoControlador : ControllerBase
    {
        private readonly AutenticacaoServico AutenticacaoServico;
        private readonly UsuarioServico UsuarioServico;
        private readonly IConfiguration _configuration;

        public AutenticacaoControlador(AutenticacaoServico AutenticacaoServico, UsuarioServico UsuarioServico, IConfiguration configuration)
        {
            this.AutenticacaoServico = AutenticacaoServico;
            this.UsuarioServico = UsuarioServico;
            _configuration = configuration;
        }

        [HttpPost("autenticar")]
        public IActionResult Autenticar([FromBody] LoginRequest request)
        {
            try
            {
                var usuario = AutenticacaoServico.Autenticar(request.Email, request.Senha)
                    ?? throw new RecursoNaoEncontradoExcecao("Email ou senha inválidos.");
                return Ok(new { usuario });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = $"Dados inválidos: {ex.Message}" });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return Unauthorized(new { mensagem = $"Erro: {ex.Message}" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensagem = "Erro inesperado ao autenticar." });
            }
        }

        [HttpPost("validar")]
        public IActionResult ValidarCredenciais([FromBody] LoginRequest request)
        {
            try
            {
                AutenticacaoServico.ValidarCredenciais(request.Email, request.Senha);
                return Ok(new { valido = true });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { valido = false, mensagem = $"Dados inválidos: {ex.Message}" });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return Unauthorized(new { valido = false, mensagem = $"Erro: {ex.Message}" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { valido = false, mensagem = "Erro inesperado ao validar credenciais." });
            }
        }

        [HttpPut("alterar-senha/{usuarioId}")]
        public IActionResult AlterarSenha(int usuarioId, [FromBody] AlterarSenhaRequest request)
        {
            try
            {
                UsuarioServico.AlterarSenha(usuarioId, request.SenhaAtual, request.SenhaNova);
                return Ok(new { sucesso = true });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { sucesso = false, mensagem = $"Usuário não encontrado: {ex.Message}" });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { sucesso = false, mensagem = $"Dados inválidos: {ex.Message}" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { sucesso = false, mensagem = "Erro inesperado ao alterar senha." });
            }
        }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Senha { get; set; }
    }

    public class AlterarSenhaRequest
    {
        public required string SenhaAtual { get; set; }
        public required string SenhaNova { get; set; }
    }
}
