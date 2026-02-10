using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class UsuarioServicoTests
{
    // Helper para criar um contexto de banco de dados em memória
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    [Fact]
    public void RegistrarCliente_ComDadosValidos_DeveAdicionarNoContexto()
    {
        // Arrange (Preparar)
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Email = "joao@teste.com",
            Senha = "senha123",
            CPF = "12345678900",
            Telefone = "11999999999",
            Endereco = "Rua Teste, 123"
        };

        // Act (Executar)
        usuarioServico.RegistrarCliente(cliente);

        // Assert (Verificar)
        var clienteCadastrado = context.Usuarios
            .OfType<Cliente>()
            .FirstOrDefault(c => c.Email == "joao@teste.com");
        
        clienteCadastrado.Should().NotBeNull();
        clienteCadastrado!.Nome.Should().Be("João Silva");
        clienteCadastrado.CPF.Should().Be("12345678900");
    }

    [Fact]
    public void RegistrarCliente_SemCPF_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        
        var cliente = new Cliente
        {
            Nome = "João Silva",
            Email = "joao@teste.com",
            Senha = "senha123",
            CPF = "", // CPF vazio
            Telefone = "11999999999",
            Endereco = "Rua Teste, 123"
        };

        // Act & Assert
        var acao = () => usuarioServico.RegistrarCliente(cliente);
        
        acao.Should().Throw<Exception>()
            .WithMessage("*CPF*");
    }
}
