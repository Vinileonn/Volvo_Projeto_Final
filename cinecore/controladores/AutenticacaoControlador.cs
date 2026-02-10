using cinecore.servicos;
using cinecore.excecoes;
using cinecore.DTOs.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace cinecore.controladores
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

        [AllowAnonymous]
        [HttpPost("autenticar")]
        public IActionResult Autenticar([FromBody] LoginRequest request)
        {
            try
            {
            var usuario = AutenticacaoServico.Autenticar(request.Email, request.Senha)
                ?? throw new RecursoNaoEncontradoExcecao("Email ou senha inválidos.");
                var token = GerarToken(usuario);
                return Ok(new { usuario, token, mensagem = "Autenticação realizada com sucesso." });
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

        [AllowAnonymous]
        [HttpPost("validar")]
        public IActionResult ValidarCredenciais([FromBody] LoginRequest request)
        {
            try
            {
                AutenticacaoServico.ValidarCredenciais(request.Email, request.Senha);
                return Ok(new { valido = true, mensagem = "Credenciais válidas." });
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

        [Authorize]
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

        private string GerarToken(cinecore.modelos.Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var role = usuario is cinecore.modelos.Administrador ? "Administrador" : "Cliente";
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim("tipo_usuario", role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpireMinutes")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
