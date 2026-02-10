using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class CinemaServicoTests
{
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    #region Testes de Criação

    [Fact]
    public void CriarCinema_ComDadosValidos_DeveAdicionarNoContexto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema
        {
            Nome = "Cine Premiun",
            Endereco = "Rua das Flores, 123, Centro"
        };

        // Act
        var resultado = cinemaServico.CriarCinema(cinema);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().BeGreaterThan(0);
        resultado.Nome.Should().Be("Cine Premiun");
        resultado.Endereco.Should().Be("Rua das Flores, 123, Centro");
        resultado.DataCriacao.Should().NotBe(DateTime.MinValue);
    }

    [Fact]
    public void CriarCinema_ComCinemaNulo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);

        // Act
        var acao = () => cinemaServico.CriarCinema(null!);

        // Assert
        acao.Should().Throw<ArgumentNullException>()
            .WithMessage("*Cinema não pode ser nulo*");
    }

    [Fact]
    public void CriarCinema_SemNome_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema
        {
            Nome = "",
            Endereco = "Rua das Flores, 123, Centro"
        };

        // Act
        var acao = () => cinemaServico.CriarCinema(cinema);

        // Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*Campos obrigatórios faltando: nome*");
    }

    [Fact]
    public void CriarCinema_SemEndereco_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema
        {
            Nome = "Cine Premiun",
            Endereco = ""
        };

        // Act
        var acao = () => cinemaServico.CriarCinema(cinema);

        // Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*Campos obrigatórios faltando: endereço*");
    }

    [Fact]
    public void CriarCinema_SemNomeEEndereco_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema
        {
            Nome = "",
            Endereco = ""
        };

        // Act
        var acao = () => cinemaServico.CriarCinema(cinema);

        // Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*Campos obrigatórios faltando: nome, endereço*");
    }

    [Fact]
    public void CriarCinema_ComNomeDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema1 = new Cinema
        {
            Nome = "Cine Premiun",
            Endereco = "Rua das Flores, 123, Centro"
        };

        var cinema2 = new Cinema
        {
            Nome = "Cine Premiun",  // Mesmo nome
            Endereco = "Rua Diferente, 456"
        };

        // Act
        cinemaServico.CriarCinema(cinema1);
        var acao = () => cinemaServico.CriarCinema(cinema2);

        // Assert
        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cinema 'Cine Premiun' já existe*");
    }

    [Fact]
    public void CriarCinema_ComNomeDuplicadoEmCasosDiferentes_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema1 = new Cinema
        {
            Nome = "CINE PREMIUN",
            Endereco = "Rua das Flores, 123, Centro"
        };

        var cinema2 = new Cinema
        {
            Nome = "cine premiun",  // Diferentes maiúsculas/minúsculas
            Endereco = "Rua Diferente, 456"
        };

        // Act
        cinemaServico.CriarCinema(cinema1);
        var acao = () => cinemaServico.CriarCinema(cinema2);

        // Assert
        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cinema 'cine premiun' já existe*");
    }

    #endregion

    #region Testes de Leitura

    [Fact]
    public void ObterCinema_ComIdValido_DeveRetornarCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema
        {
            Nome = "Cine Premiun",
            Endereco = "Rua das Flores, 123, Centro"
        };

        cinemaServico.CriarCinema(cinema);

        // Act
        var resultado = cinemaServico.ObterCinema(cinema.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Cine Premiun");
        resultado.Endereco.Should().Be("Rua das Flores, 123, Centro");
    }

    [Fact]
    public void ObterCinema_ComIdInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);

        // Act
        var acao = () => cinemaServico.ObterCinema(999);

        // Assert
        acao.Should().Throw<KeyNotFoundException>()
            .WithMessage("*Cinema com ID 999 não encontrado*");
    }

    [Fact]
    public void ListarCinemas_SemCinemas_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);

        // Act
        var resultado = cinemaServico.ListarCinemas();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public void ListarCinemas_ComMultiplosCinemas_DeveRetornarTodos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema1 = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        var cinema2 = new Cinema { Nome = "Cine Art", Endereco = "Rua 2" };
        var cinema3 = new Cinema { Nome = "Cine Max", Endereco = "Rua 3" };

        cinemaServico.CriarCinema(cinema1);
        cinemaServico.CriarCinema(cinema2);
        cinemaServico.CriarCinema(cinema3);

        // Act
        var resultado = cinemaServico.ListarCinemas();

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Should().Contain(c => c.Nome == "Cine Premiun");
        resultado.Should().Contain(c => c.Nome == "Cine Art");
        resultado.Should().Contain(c => c.Nome == "Cine Max");
    }

    [Fact]
    public void BuscarPorNome_ComNomeExistente_DeveRetornarCinemas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema1 = new Cinema { Nome = "Cine Premiun Centro", Endereco = "Rua 1" };
        var cinema2 = new Cinema { Nome = "Cine Art", Endereco = "Rua 2" };
        var cinema3 = new Cinema { Nome = "Cine Premiun Norte", Endereco = "Rua 3" };

        cinemaServico.CriarCinema(cinema1);
        cinemaServico.CriarCinema(cinema2);
        cinemaServico.CriarCinema(cinema3);

        // Act
        var resultado = cinemaServico.BuscarPorNome("Premiun");

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().AllSatisfy(c => c.Nome.Should().Contain("Premiun"));
    }

    [Fact]
    public void BuscarPorNome_ComNomeNaoExistente_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        // Act
        var resultado = cinemaServico.BuscarPorNome("Inexistente");

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public void BuscarPorNome_ComNomeNuloOuVazio_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        // Act
        var resultadoNulo = cinemaServico.BuscarPorNome(null!);
        var resultadoVazio = cinemaServico.BuscarPorNome("");
        var resultadoEspacos = cinemaServico.BuscarPorNome("   ");

        // Assert
        resultadoNulo.Should().BeEmpty();
        resultadoVazio.Should().BeEmpty();
        resultadoEspacos.Should().BeEmpty();
    }

    [Fact]
    public void BuscarPorNome_CaseSensitive_DeveRetornarCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine PREMIUN", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        // Act
        var resultado1 = cinemaServico.BuscarPorNome("premiun");
        var resultado2 = cinemaServico.BuscarPorNome("PREMIUN");
        var resultado3 = cinemaServico.BuscarPorNome("Premiun");

        // Assert
        resultado1.Should().HaveCount(1);
        resultado2.Should().HaveCount(1);
        resultado3.Should().HaveCount(1);
    }

    #endregion

    #region Testes de Atualização

    [Fact]
    public void AtualizarCinema_ComDadosValidos_DeveAtualizarCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        var cinemaAtualizado = new Cinema 
        { 
            Nome = "Cine Premier Atualizado",
            Endereco = "Rua 2 Atualizada"
        };

        // Act
        var resultado = cinemaServico.AtualizarCinema(cinema.Id, cinemaAtualizado);

        // Assert
        resultado.Nome.Should().Be("Cine Premier Atualizado");
        resultado.Endereco.Should().Be("Rua 2 Atualizada");
        resultado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarCinema_ApenasNome_DeveAtualizarApenasoNome()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        var cinemaAtualizado = new Cinema 
        { 
            Nome = "Cine Premier Novo",
            Endereco = ""  // Vazio, não deve atualizar
        };

        // Act
        var resultado = cinemaServico.AtualizarCinema(cinema.Id, cinemaAtualizado);

        // Assert
        resultado.Nome.Should().Be("Cine Premier Novo");
        resultado.Endereco.Should().Be("Rua 1");  // Mantém o original
    }

    [Fact]
    public void AtualizarCinema_ApenasEndereco_DeveAtualizarApenasEndereco()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        var cinemaAtualizado = new Cinema 
        { 
            Nome = "",  // Vazio, não deve atualizar
            Endereco = "Rua 2 Nova"
        };

        // Act
        var resultado = cinemaServico.AtualizarCinema(cinema.Id, cinemaAtualizado);

        // Assert
        resultado.Nome.Should().Be("Cine Premiun");  // Mantém o original
        resultado.Endereco.Should().Be("Rua 2 Nova");
    }

    [Fact]
    public void AtualizarCinema_ComNomeDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema1 = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        var cinema2 = new Cinema { Nome = "Cine Art", Endereco = "Rua 2" };
        
        cinemaServico.CriarCinema(cinema1);
        cinemaServico.CriarCinema(cinema2);

        var cinemaAtualizado = new Cinema 
        { 
            Nome = "Cine Premiun",  // Mesmo nome do cinema1
            Endereco = "Rua 2"
        };

        // Act
        var acao = () => cinemaServico.AtualizarCinema(cinema2.Id, cinemaAtualizado);

        // Assert
        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cinema 'Cine Premiun' já existe*");
    }

    [Fact]
    public void AtualizarCinema_ComIdNaoExistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinemaAtualizado = new Cinema 
        { 
            Nome = "Cinema Novo",
            Endereco = "Nova Rua"
        };

        // Act
        var acao = () => cinemaServico.AtualizarCinema(999, cinemaAtualizado);

        // Assert
        acao.Should().Throw<KeyNotFoundException>();
    }

    #endregion

    #region Testes de Exclusão

    [Fact]
    public void DeletarCinema_ComIdValido_DeveDeletarCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        // Act
        cinemaServico.DeletarCinema(cinema.Id);

        // Assert
        var acao = () => cinemaServico.ObterCinema(cinema.Id);
        acao.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void DeletarCinema_ComIdNaoExistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);

        // Act
        var acao = () => cinemaServico.DeletarCinema(999);

        // Assert
        acao.Should().Throw<KeyNotFoundException>();
    }

    #endregion

    #region Testes de Operações com Relacionamentos

    [Fact]
    public void ObterSalasDoCinema_ComSalas_DeveRetornarSalas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        var sala1 = new Sala { Nome = "Sala 1", Capacidade = 100, Cinema = cinema };
        var sala2 = new Sala { Nome = "Sala 2", Capacidade = 150, Cinema = cinema };

        context.Salas.Add(sala1);
        context.Salas.Add(sala2);
        context.SaveChanges();

        // Act
        var resultado = cinemaServico.ObterSalasDoCinema(cinema.Id);

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public void ObterSalasDoCinema_SemSalas_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        // Act
        var resultado = cinemaServico.ObterSalasDoCinema(cinema.Id);

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public void ObterFuncionariosDoCinema_ComFuncionarios_DeveRetornarFuncionarios()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        var funcionario1 = new Funcionario 
        { 
            Nome = "João Silva",
            Cargo = Enums.CargoFuncionario.Gerente,
            Cinema = cinema
        };
        var funcionario2 = new Funcionario 
        { 
            Nome = "Maria Santos",
            Cargo = Enums.CargoFuncionario.AtendenteAlimentos,
            Cinema = cinema
        };

        context.Funcionarios.Add(funcionario1);
        context.Funcionarios.Add(funcionario2);
        context.SaveChanges();

        // Act
        var resultado = cinemaServico.ObterFuncionariosDoCinema(cinema.Id);

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public void ObterFuncionariosDoCinema_SemFuncionarios_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var cinemaServico = new CinemaServico(context);
        
        var cinema = new Cinema { Nome = "Cine Premiun", Endereco = "Rua 1" };
        cinemaServico.CriarCinema(cinema);

        // Act
        var resultado = cinemaServico.ObterFuncionariosDoCinema(cinema.Id);

        // Assert
        resultado.Should().BeEmpty();
    }

    #endregion
}

