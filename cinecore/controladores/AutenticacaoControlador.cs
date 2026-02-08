using cinecore.modelos;
using cinecore.servicos;
using cinecore.excecoes;
using Microsoft.AspNetCore.Mvc;

namespace cinecore.controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoControlador : ControllerBase
    {
        private readonly AutenticacaoServico AutenticacaoServico;
        private readonly UsuarioServico UsuarioServico;

        public AutenticacaoControlador(AutenticacaoServico AutenticacaoServico, UsuarioServico UsuarioServico)
        {
            this.AutenticacaoServico = AutenticacaoServico;
            this.UsuarioServico = UsuarioServico;
        }

        [HttpPost("autenticar")]
        public IActionResult Autenticar([FromBody] LoginRequest request)
        {
            try
            {
                var usuario = AutenticacaoServico.Autenticar(request.Email, request.Senha);
                return Ok(new { usuario, mensagem = "Autenticação realizada com sucesso." });
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

        [HttpPost("registrar")]
        public IActionResult RegistrarCliente([FromBody] Cliente cliente)
        {
            try
            {
                UsuarioServico.RegistrarCliente(cliente);
                return Ok(new { sucesso = true, mensagem = "Cadastro realizado com sucesso." });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { sucesso = false, mensagem = $"Dados inválidos: {ex.Message}" });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { sucesso = false, mensagem = $"Operação não permitida: {ex.Message}" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { sucesso = false, mensagem = "Erro inesperado ao cadastrar cliente." });
            }
        }

        [HttpPut("alterar-senha/{usuarioId}")]
        public IActionResult AlterarSenha(int usuarioId, [FromBody] AlterarSenhaRequest request)
        {
            try
            {
                UsuarioServico.AlterarSenha(usuarioId, request.SenhaAtual, request.SenhaNova);
                return Ok(new { sucesso = true, mensagem = "Senha alterada com sucesso." });
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
