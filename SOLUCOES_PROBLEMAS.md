# CineFlow - Soluções para Problemas Críticos

## 1. 🔐 Implementar JWT Authentication

### Problema
Nenhum endpoint está protegido. Qualquer cliente pode fazer operações administrativas.

### Solução

#### 1.1 Instalar Package
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
```

#### 1.2 Configurar no Program.cs
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ... existing code ...

// Configuração de autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT Key não configurada");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// IMPORTANTE: Adicionar antes de MapControllers
app.UseAuthentication();
app.UseAuthorization();
```

#### 1.3 Atualizar appsettings.json
```json
{
  "Jwt": {
    "Key": "sua_chave_secreta_muito_longa_pelo_menos_32_caracteres_aqui",
    "Issuer": "CineFlowAPI",
    "Audience": "CineFlowClient",
    "ExpirationMinutes": 60
  },
  "Admin": {
    "Email": "admin@cinema.com",
    "Senha": "SenhaTemporaria123!@#"
  }
}
```

#### 1.4 Criar Serviço de Token
```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace cinecore.servicos
{
    public class TokenServico
    {
        private readonly IConfiguration _configuration;

        public TokenServico(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(int usuarioId, string email, string tipo)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, tipo) // "Admin", "Cliente", "Funcionario"
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
```

#### 1.5 Criar DTO de Autenticação
```csharp
namespace cinecore.DTOs.Autenticacao
{
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Senha { get; set; }
    }

    public class TokenResponseDto
    {
        public required string Token { get; set; }
        public required string TipoUsuario { get; set; }
        public required int UsuarioId { get; set; }
    }
}
```

#### 1.6 Criar Endpoint de Login
```csharp
using Microsoft.AspNetCore.Mvc;
using cinecore.DTOs.Autenticacao;
using cinecore.servicos;
using cinecore.modelos;

namespace cinecore.controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoControlador : ControllerBase
    {
        private readonly AutenticacaoServico _autenticacaoServico;
        private readonly TokenServico _tokenServico;

        public AutenticacaoControlador(AutenticacaoServico autenticacaoServico, TokenServico tokenServico)
        {
            _autenticacaoServico = autenticacaoServico;
            _tokenServico = tokenServico;
        }

        [HttpPost("Login")]
        public ActionResult<TokenResponseDto> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var usuario = _autenticacaoServico.Autenticar(loginDto.Email, loginDto.Senha);
                
                var tipoUsuario = usuario switch
                {
                    Administrador => "Admin",
                    Cliente => "Cliente",
                    Funcionario => "Funcionario",
                    _ => "Usuario"
                };

                var token = _tokenServico.GerarToken(usuario.Id, usuario.Email, tipoUsuario);

                return Ok(new TokenResponseDto
                {
                    Token = token,
                    TipoUsuario = tipoUsuario,
                    UsuarioId = usuario.Id
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { mensagem = "Email ou senha inválidos" });
            }
        }
    }
}
```

#### 1.7 Proteger Controllers com [Authorize]
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cinecore.controladores
{
    [Authorize]  // Requer token válido
    [ApiController]
    [Route("api/[controller]")]
    public class FilmeControlador : ControllerBase
    {
        [HttpPost("Criar")]
        [Authorize(Roles = "Admin")]  // Apenas Admin pode criar
        public ActionResult<FilmeDto> CriarFilme([FromBody] CriarFilmeDto criarFilmeDto)
        {
            // ... implementação
        }

        [HttpGet]
        [AllowAnonymous]  // Permitir consulta
        public ActionResult<IEnumerable<FilmeDto>> ListarFilmes()
        {
            // ... implementação
        }
    }
}
```

---

## 2. 🔒 Hash de Senhas com Bcrypt

### Problema
Senhas armazenadas em plain text são uma vulnerabilidade crítica.

### Solução

#### 2.1 Instalar BCrypt
```bash
dotnet add package BCrypt.Net-Core
```

#### 2.2 Criar Serviço de Hash
```csharp
using BCrypt.Net;

namespace cinecore.utilitarios
{
    public static class SenhaUtilitario
    {
        public static string HashSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha não pode ser vazia");

            return BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 12);
        }

        public static bool VerificarSenha(string senhaPlain, string senhaHash)
        {
            if (string.IsNullOrWhiteSpace(senhaPlain) || string.IsNullOrWhiteSpace(senhaHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(senhaPlain, senhaHash);
        }
    }
}
```

#### 2.3 Atualizar Modelo Usuario
```csharp
namespace cinecore.modelos
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        
        // ANTES: public string Senha { get; set; }
        // DEPOIS: Armazenar hash
        public required string SenhaHash { get; set; }  // Armazena hash, não a senha
        
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        // Método para definir senha (hash automaticamente)
        public void DefinirSenha(string senhaPlain)
        {
            SenhaHash = SenhaUtilitario.HashSenha(senhaPlain);
        }

        // Método para verificar senha
        public bool VerificarSenha(string senhaPlain)
        {
            return SenhaUtilitario.VerificarSenha(senhaPlain, SenhaHash);
        }
    }
}
```

#### 2.4 Atualizar UsuarioServico
```csharp
public Usuario? ObterUsuarioPorCredenciais(string email, string senha)
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        return null;

    var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
    
    if (usuario == null)
        return null;

    // Usar VerificarSenha em vez de comparação direta
    if (usuario.VerificarSenha(senha))
        return usuario;

    return null;
}
```

---

## 3. 🧪 Adicionar Testes Unitários

### Problema
Sem testes, impossível validar mudanças com segurança.

### Solução

#### 3.1 Criar Projeto de Testes
```bash
dotnet new xunit -n CineCore.Tests
cd CineCore.Tests
dotnet add package Moq
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add reference ../cinecore/cinecore.csproj
```

#### 3.2 Teste para IngressoServico
```csharp
using Xunit;
using Moq;
using CineCore.Servicos;
using CineCore.Modelos;
using CineCore.Dados;
using Microsoft.EntityFrameworkCore;

namespace CineCore.Tests.Servicos
{
    public class IngressoServicoTests
    {
        private CineFlowContext CriarContextoEmMemoria()
        {
            var options = new DbContextOptionsBuilder<CineFlowContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new CineFlowContext(options);
        }

        [Fact]
        public void VenderInteira_ComClienteValido_DeveCriarIngresso()
        {
            // Arrange
            var context = CriarContextoEmMemoria();
            var servico = new IngressoServico(context);

            var sala = new Sala { Id = 1, Nome = "Sala 1", Capacidade = 50, Tipo = TipoSala.Normal };
            var filme = new Filme { Id = 1, Titulo = "Filme Teste" };
            var sessao = new Sessao 
            { 
                Id = 1, 
                DataHora = DateTime.Now.AddHours(2),
                PrecoFinal = 50.0f, 
                Sala = sala, 
                Filme = filme 
            };
            var cliente = new Cliente 
            { 
                Id = 1, 
                Nome = "Cliente Teste",
                Email = "cliente@test.com",
                CPF = "12345678901",
                Telefone = "11999999999",
                Endereco = "Rua Teste",
                DataNascimento = DateTime.Now.AddYears(-25)
            };

            context.Salas.Add(sala);
            context.Filmes.Add(filme);
            context.Sessoes.Add(sessao);
            context.Clientes.Add(cliente);
            context.SaveChanges();

            // Act
            var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Dinheiro);

            // Assert
            Assert.NotNull(ingresso);
            Assert.Equal('A', ingresso.Fila);
            Assert.Equal(1, ingresso.Numero);
        }

        [Fact]
        public void VenderInteira_ComAssentoOcupado_LancaExcecao()
        {
            // Arrange
            var context = CriarContextoEmMemoria();
            var servico = new IngressoServico(context);

            // Já existe um ingresso para A1
            // Act & Assert
            Assert.Throws<OperacaoNaoPermitidaExcecao>(() => 
            {
                servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Dinheiro);
            });
        }
    }
}
```

#### 3.3 Teste para Controller com Mock
```csharp
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CineCore.Controladores;
using CineCore.Servicos;
using CineCore.DTOs.Filme;

namespace CineCore.Tests.Controladores
{
    public class FilmeControladorTests
    {
        [Fact]
        public void ListarFilmes_DeveRetornarOk()
        {
            // Arrange
            var mockServico = new Mock<FilmeServico>();
            var mockMapper = new Mock<IMapper>();

            mockServico.Setup(s => s.ListarFilmes())
                .Returns(new List<Filme> { new Filme { Id = 1, Titulo = "Filme 1" } });

            mockMapper.Setup(m => m.Map<List<FilmeDto>>(It.IsAny<List<Filme>>()))
                .Returns(new List<FilmeDto> { new FilmeDto { Id = 1, Titulo = "Filme 1" } });

            var controller = new FilmeControlador(mockServico.Object, mockMapper.Object);

            // Act
            var resultado = controller.ListarFilmes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var filmes = Assert.IsType<List<FilmeDto>>(okResult.Value);
            Assert.Single(filmes);
        }
    }
}
```

---

## 4. 📊 Implementar Logging com Serilog

### Problema
Não há logs, impossível rastrear erros em produção.

### Solução

#### 4.1 Instalar Serilog
```bash
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
```

#### 4.2 Configurar no Program.cs
```csharp
using Serilog;

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/cineflow-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog();  // Integrar Serilog
    
    // ... rest of configuration
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

#### 4.3 Usar Logging em Controllers
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilmeControlador : ControllerBase
{
    private readonly FilmeServico _filmeServico;
    private readonly ILogger<FilmeControlador> _logger;

    public FilmeControlador(FilmeServico filmeServico, ILogger<FilmeControlador> logger)
    {
        _filmeServico = filmeServico;
        _logger = logger;
    }

    [HttpPost("Criar")]
    public ActionResult<FilmeDto> CriarFilme([FromBody] CriarFilmeDto dto)
    {
        try
        {
            _logger.LogInformation("Tentativa de criar filme: {Titulo}", dto.Titulo);
            
            var filme = _mapper.Map<Filme>(dto);
            var novoFilme = _filmeServico.CriarFilme(filme);
            
            _logger.LogInformation("Filme criado com sucesso. ID: {FilmeId}", novoFilme.Id);
            
            return CreatedAtAction(nameof(ObterFilme), new { id = novoFilme.Id }, novoFilme);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Erro ao criar filme: {Mensagem}", ex.Message);
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar filme");
            return StatusCode(500, new { mensagem = "Erro ao processar solicitação" });
        }
    }
}
```

---

## 5. 📄 Middleware de Tratamento de Erros Global

```csharp
namespace cinecore.middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado em {Path}", context.Request.Path);
                
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new
                {
                    mensagem = "Erro ao processar solicitação",
                    status = context.Response.StatusCode,
                    // NÃO incluir stack trace em produção
                    detalhes = ex.Message
                });
            }
        }
    }
}
```

Adicionar no `Program.cs`:
```csharp
app.UseMiddleware<ErrorHandlingMiddleware>();
```

---

## Checklist de Implementação

```
[ ] 1. Adicionar JWT Authentication
[ ] 2. Configurar TokenServico
[ ] 3. Proteger Controllers com [Authorize]
[ ] 4. Implementar Bcrypt para senhas
[ ] 5. Migração de banco para SenhaHash
[ ] 6. Criar projeto de testes
[ ] 7. Escrever testes unitários (mín. 15)
[ ] 8. Implementar Serilog
[ ] 9. Adicionar ILogger em todos Controllers
[ ] 10. Criar ErrorHandlingMiddleware
[ ] 11. Testar endpoints com Postman/Insomnia
[ ] 12. Review de segurança
```

---

**Estimativa de Trabalho:** 40-50 horas (1-2 sprints)
