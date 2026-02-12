using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class AutenticacaoServicoTests
{
    // Helper para criar um contexto de banco de dados em memória
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    private static Administrador CriarAdministradorPadrao(
        string? email = null,
        string? nome = null,
        string? senha = null)
    {
        return new Administrador
        {
            Nome = nome ?? "Administrador",
            Email = email ?? "admin@teste.com",
            Senha = senha ?? "senha123"
        };
    }

    private static Cliente CriarClientePadrao(
        string? email = null,
        string? cpf = null,
        string? nome = null,
        string? senha = null)
    {
        return new Cliente
        {
            Nome = nome ?? "Joao Silva",
            Email = email ?? "joao@teste.com",
            Senha = senha ?? "senha123",
            CPF = cpf ?? "12345678900",
            Telefone = "11999999999",
            Endereco = "Rua Teste, 123",
            DataNascimento = new DateTime(1995, 5, 10)
        };
    }

    // ===== Testes para método Autenticar =====

    [Fact]
    // Deve retornar usuário
    public void AutenticarComCredenciaisValidasAdministrador()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        var admin = CriarAdministradorPadrao(email: "admin@teste.com", senha: "senha123");
        usuarioServico.RegistrarAdministrador(admin);

        // Act
        var usuarioAutenticado = autenticacaoServico.Autenticar("admin@teste.com", "senha123");

        // Assert
        usuarioAutenticado.Should().NotBeNull();
        usuarioAutenticado!.Email.Should().Be("admin@teste.com");
        usuarioAutenticado.Nome.Should().Be("Administrador");
    }

    [Fact]
    // Deve retornar usuário
    public void AutenticarComCredenciaisValidasCliente()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        var cliente = CriarClientePadrao(email: "client@teste.com", senha: "minhaSenha123");
        usuarioServico.RegistrarCliente(cliente);

        // Act
        var usuarioAutenticado = autenticacaoServico.Autenticar("client@teste.com", "minhaSenha123");

        // Assert
        usuarioAutenticado.Should().NotBeNull();
        usuarioAutenticado!.Email.Should().Be("client@teste.com");
        usuarioAutenticado.Nome.Should().Be("Joao Silva");
    }

    [Fact]
    // Deve lançar DadosInvalidosExcecao
    public void AutenticarComEmailVazio()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<DadosInvalidosExcecao>(
            () => autenticacaoServico.Autenticar("", "senha123"));
        
        excecao.Message.Should().Contain("Email ou senha não informados");
    }

    [Fact]
    // Deve lançar DadosInvalidosExcecao
    public void AutenticarComSenhaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<DadosInvalidosExcecao>(
            () => autenticacaoServico.Autenticar("admin@teste.com", ""));
        
        excecao.Message.Should().Contain("Email ou senha não informados");
    }

    [Fact]
    // Deve lançar DadosInvalidosExcecao
    public void AutenticarComEmailNulo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<DadosInvalidosExcecao>(
            () => autenticacaoServico.Autenticar(null!, "senha123"));
        
        excecao.Message.Should().Contain("Email ou senha não informados");
    }

    [Fact]
    // Deve lançar DadosInvalidosExcecao
    public void AutenticarComSenhaNula()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<DadosInvalidosExcecao>(
            () => autenticacaoServico.Autenticar("admin@teste.com", null!));
        
        excecao.Message.Should().Contain("Email ou senha não informados");
    }

    [Fact]
    // Deve lançar RecursoNaoEncontradoExcecao
    public void AutenticarComCredenciaisInvalidas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        var admin = CriarAdministradorPadrao(email: "admin@teste.com", senha: "senhaCorreta");
        usuarioServico.RegistrarAdministrador(admin);

        // Act & Assert
        var excecao = Assert.Throws<RecursoNaoEncontradoExcecao>(
            () => autenticacaoServico.Autenticar("admin@teste.com", "senhaErrada"));
        
        excecao.Message.Should().Contain("Email ou senha inválidos");
    }

    [Fact]
    // Deve lançar RecursoNaoEncontradoExcecao
    public void AutenticarComEmailInexistente()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<RecursoNaoEncontradoExcecao>(
            () => autenticacaoServico.Autenticar("naoexiste@teste.com", "senha123"));
        
        excecao.Message.Should().Contain("Email ou senha inválidos");
    }

    [Fact]
    // Deve autenticar com sucesso
    public void AutenticarComEmailMaiusculasDiferente()
    {
        // Arrange - O sistema deve fazer comparação case-insensitive para email
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        var admin = CriarAdministradorPadrao(email: "Admin@Teste.Com", senha: "senha123");
        usuarioServico.RegistrarAdministrador(admin);

        // Act
        var usuarioAutenticado = autenticacaoServico.Autenticar("admin@teste.com", "senha123");

        // Assert
        usuarioAutenticado.Should().NotBeNull();
        usuarioAutenticado!.Email.Should().Be("Admin@Teste.Com");
    }

    // ===== Testes para método ValidarCredenciais =====

    [Fact]
    // Não deve lançar exceção
    public void ValidarCredenciaisComCredenciaisValidas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        var admin = CriarAdministradorPadrao(email: "admin@teste.com", senha: "senha123");
        usuarioServico.RegistrarAdministrador(admin);

        // Act & Assert
        var excecao = Record.Exception(
            () => autenticacaoServico.ValidarCredenciais("admin@teste.com", "senha123"));
        
        excecao.Should().BeNull();
    }

    [Fact]
    // Deve lançar DadosInvalidosExcecao
    public void ValidarCredenciaisComEmailVazio()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<DadosInvalidosExcecao>(
            () => autenticacaoServico.ValidarCredenciais("", "senha123"));
        
        excecao.Message.Should().Contain("Email ou senha não informados");
    }

    [Fact]
    // Deve lançar DadosInvalidosExcecao
    public void ValidarCredenciaisComSenhaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<DadosInvalidosExcecao>(
            () => autenticacaoServico.ValidarCredenciais("admin@teste.com", ""));
        
        excecao.Message.Should().Contain("Email ou senha não informados");
    }

    [Fact]
    // Deve lançar RecursoNaoEncontradoExcecao
    public void ValidarCredenciaisComCredenciaisInvalidas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        var admin = CriarAdministradorPadrao(email: "admin@teste.com", senha: "senhaCorreta");
        usuarioServico.RegistrarAdministrador(admin);

        // Act & Assert
        var excecao = Assert.Throws<RecursoNaoEncontradoExcecao>(
            () => autenticacaoServico.ValidarCredenciais("admin@teste.com", "senhaErrada"));
        
        excecao.Message.Should().Contain("Email ou senha inválidos");
    }

    [Fact]
    // Deve lançar RecursoNaoEncontradoExcecao
    public void ValidarCredenciaisComEmailInexistente()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<RecursoNaoEncontradoExcecao>(
            () => autenticacaoServico.ValidarCredenciais("naoexiste@teste.com", "senha123"));
        
        excecao.Message.Should().Contain("Email ou senha inválidos");
    }

    [Fact]
    // Deve validar com sucesso
    public void ValidarCredenciaisComEmailMaiusculasDiferente()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        var cliente = CriarClientePadrao(email: "Cliente@Teste.Com", senha: "senha123");
        usuarioServico.RegistrarCliente(cliente);

        // Act & Assert
        var excecao = Record.Exception(
            () => autenticacaoServico.ValidarCredenciais("cliente@teste.com", "senha123"));
        
        excecao.Should().BeNull();
    }

    [Fact]
    // Deve lançar DadosInvalidosExcecao
    public void ValidarCredenciaisComMultiplosCamposVazios()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        var autenticacaoServico = new AutenticacaoServico(usuarioServico);

        // Act & Assert
        var excecao = Assert.Throws<DadosInvalidosExcecao>(
            () => autenticacaoServico.ValidarCredenciais("   ", "   "));
        
        excecao.Message.Should().Contain("Email ou senha não informados");
    }
}
