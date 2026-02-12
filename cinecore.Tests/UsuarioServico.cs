using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
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

    [Fact]
    public void RegistrarClienteComDadosValidos()
    {
        // Arrange (Preparar)
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        
        var cliente = CriarClientePadrao(nome: "Joao Silva");

        // Act (Executar)
        usuarioServico.RegistrarCliente(cliente);

        // Assert (Verificar)
        var clienteCadastrado = context.Usuarios
            .OfType<Cliente>()
            .FirstOrDefault(c => c.Email == "joao@teste.com");
        
        clienteCadastrado.Should().NotBeNull();
        clienteCadastrado!.Nome.Should().Be("Joao Silva");
        clienteCadastrado.CPF.Should().Be("12345678900");
    }

    [Fact]
    public void RegistrarClienteSemCPF()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);
        
        var cliente = CriarClientePadrao(cpf: "");

        // Act & Assert
        var acao = () => usuarioServico.RegistrarCliente(cliente);
        
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*CPF*");
    }

    [Fact]
    public void RegistrarClienteComEmailDuplicado()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        var cliente1 = CriarClientePadrao(email: "duplicado@teste.com", cpf: "11111111111");
        var cliente2 = CriarClientePadrao(email: "duplicado@teste.com", cpf: "22222222222");

        usuarioServico.RegistrarCliente(cliente1);

        // Act
        var acao = () => usuarioServico.RegistrarCliente(cliente2);

        // Assert
        acao.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("*Email*já cadastrado*");
    }

    [Fact]
    public void RegistrarClienteComCpfDuplicado()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        var cliente1 = CriarClientePadrao(email: "cliente1@teste.com", cpf: "33333333333");
        var cliente2 = CriarClientePadrao(email: "cliente2@teste.com", cpf: "33333333333");

        usuarioServico.RegistrarCliente(cliente1);

        // Act
        var acao = () => usuarioServico.RegistrarCliente(cliente2);

        // Assert
        acao.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("*CPF*já cadastrado*");
    }

    [Fact]
    public void RegistrarAdministradorComDadosValidos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        var admin = CriarAdministradorPadrao();

        // Act
        usuarioServico.RegistrarAdministrador(admin);

        // Assert
        var adminCadastrado = context.Administradores.FirstOrDefault(a => a.Email == "admin@teste.com");
        adminCadastrado.Should().NotBeNull();
        adminCadastrado!.Nome.Should().Be("Administrador");
    }

    [Fact]
    public void RegistrarAdministradorComEmailDuplicado()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        var cliente = CriarClientePadrao(email: "admin@teste.com", cpf: "44444444444");
        usuarioServico.RegistrarCliente(cliente);

        var admin = CriarAdministradorPadrao(email: "admin@teste.com");

        // Act
        var acao = () => usuarioServico.RegistrarAdministrador(admin);

        // Assert
        acao.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("*Email*já cadastrado*");
    }

    [Fact]
    public void ObterUsuarioComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        // Act
        var acao = () => usuarioServico.ObterUsuario(999);

        // Assert
        acao.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public void ObterUsuarioPorEmailEmailEmBranco()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        // Act
        var acao = () => usuarioServico.ObterUsuarioPorEmail(" ");

        // Assert
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*Email não informado*");
    }

    [Fact]
    public void AtualizarUsuarioComEmailJaEmUso()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        var cliente1 = CriarClientePadrao(email: "cliente1@teste.com", cpf: "55555555555");
        var cliente2 = CriarClientePadrao(email: "cliente2@teste.com", cpf: "66666666666");

        usuarioServico.RegistrarCliente(cliente1);
        usuarioServico.RegistrarCliente(cliente2);

        // Act
        var acao = () => usuarioServico.AtualizarUsuario(cliente1.Id, email: "cliente2@teste.com");

        // Assert
        acao.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("*Email*já está em uso*");
    }

    [Fact]
    public void DeletarUsuarioAdmin()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        var admin = CriarAdministradorPadrao();
        usuarioServico.RegistrarAdministrador(admin);

        // Act
        var acao = () => usuarioServico.DeletarUsuario(admin.Id);

        // Assert
        acao.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("*Não é permitido deletar o usuário Administrador*" );
    }

    [Fact]
    public void AlterarSenhaComSenhaAtualIncorreta()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        var cliente = CriarClientePadrao(email: "senha@teste.com", cpf: "77777777777");
        usuarioServico.RegistrarCliente(cliente);

        // Act
        var acao = () => usuarioServico.AlterarSenha(cliente.Id, "senhaerrada", "novaSenha123");

        // Assert
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*senha atual incorreta*");
    }

    [Fact]
    public void AlterarSenhaComDadosValidos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var usuarioServico = new UsuarioServico(context);

        var cliente = CriarClientePadrao(email: "alterar@teste.com", cpf: "88888888888", senha: "senhaAntiga");
        usuarioServico.RegistrarCliente(cliente);

        // Act
        usuarioServico.AlterarSenha(cliente.Id, "senhaAntiga", "senhaNova");

        // Assert
        var atualizado = usuarioServico.ObterUsuario(cliente.Id);
        atualizado.Senha.Should().Be("senhaNova");
    }
}
