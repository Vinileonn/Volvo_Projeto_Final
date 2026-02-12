using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class FuncionarioServicoTests
{
    // Helper para criar um contexto de banco de dados em memória
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    private static Cinema CriarCinemaPadrao(
        int id = 1,
        string? nome = null,
        string? endereco = null)
    {
        return new Cinema
        {
            Id = id,
            Nome = nome ?? "Cinema Teste",
            Endereco = endereco ?? "Rua Teste, 123"
        };
    }

    private static Funcionario CriarFuncionarioPadrao(
        int id = 0,
        string? nome = null,
        CargoFuncionario cargo = CargoFuncionario.AtendenteIngressos,
        Cinema? cinema = null)
    {
        return new Funcionario
        {
            Id = id,
            Nome = nome ?? "João Silva",
            Cargo = cargo,
            Cinema = cinema ?? CriarCinemaPadrao()
        };
    }

    #region Testes CriarFuncionario

    [Fact]
    public void CriarFuncionarioComDadosValidos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();
        var funcionario = CriarFuncionarioPadrao(nome: "Maria Santos", cargo: CargoFuncionario.Gerente, cinema: cinema);

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        // Act
        servico.CriarFuncionario(funcionario);

        // Assert
        var funcionarioDb = context.Funcionarios.FirstOrDefault(f => f.Id == funcionario.Id);
        funcionarioDb.Should().NotBeNull();
        funcionarioDb!.Nome.Should().Be("Maria Santos");
        funcionarioDb.Cargo.Should().Be(CargoFuncionario.Gerente);
        funcionarioDb.Cinema.Should().Be(cinema);
        funcionarioDb.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CriarFuncionarioComFuncionarioNulo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);

        // Act
        Action act = () => servico.CriarFuncionario(null!);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Funcionario nao pode ser nulo.");
    }

    [Fact]
    public void CriarFuncionarioComNomeVazio()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();
        var funcionario = CriarFuncionarioPadrao(cinema: cinema);
        funcionario.Nome = "";

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        // Act
        Action act = () => servico.CriarFuncionario(funcionario);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Nome do funcionario e obrigatorio.");
    }

    [Fact]
    public void CriarFuncionarioComNomeNulo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();
        var funcionario = new Funcionario
        {
            Nome = null!,
            Cargo = CargoFuncionario.Limpeza,
            Cinema = cinema
        };

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        // Act
        Action act = () => servico.CriarFuncionario(funcionario);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Nome do funcionario e obrigatorio.");
    }

    [Fact]
    public void CriarFuncionarioComNomeApenasEspacos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();
        var funcionario = CriarFuncionarioPadrao(cinema: cinema);
        funcionario.Nome = "   ";

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        // Act
        Action act = () => servico.CriarFuncionario(funcionario);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Nome do funcionario e obrigatorio.");
    }

    [Fact]
    public void CriarFuncionarioSemCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var funcionario = CriarFuncionarioPadrao();
        funcionario.Cinema = null;

        // Act
        Action act = () => servico.CriarFuncionario(funcionario);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Cinema do funcionario e obrigatorio.");
    }

    [Fact]
    public void CriarFuncionarioComDiferentesCargos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var cargos = new[]
        {
            CargoFuncionario.AtendenteIngressos,
            CargoFuncionario.AtendenteAlimentos,
            CargoFuncionario.Limpeza,
            CargoFuncionario.Garcom,
            CargoFuncionario.Gerente
        };

        // Act & Assert
        foreach (var cargo in cargos)
        {
            var funcionario = CriarFuncionarioPadrao(cargo: cargo, cinema: cinema);
            servico.CriarFuncionario(funcionario);
            
            var funcionarioDb = context.Funcionarios.FirstOrDefault(f => f.Id == funcionario.Id);
            funcionarioDb.Should().NotBeNull();
            funcionarioDb!.Cargo.Should().Be(cargo);
        }

        context.Funcionarios.Should().HaveCount(cargos.Length);
    }

    #endregion

    #region Testes ObterFuncionario

    [Fact]
    public void ObterFuncionarioComIdValido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();
        var funcionario = CriarFuncionarioPadrao(cinema: cinema);

        context.Cinemas.Add(cinema);
        context.SaveChanges();
        servico.CriarFuncionario(funcionario);

        // Act
        var funcionarioObtido = servico.ObterFuncionario(funcionario.Id);

        // Assert
        funcionarioObtido.Should().NotBeNull();
        funcionarioObtido.Id.Should().Be(funcionario.Id);
        funcionarioObtido.Nome.Should().Be(funcionario.Nome);
        funcionarioObtido.Cargo.Should().Be(funcionario.Cargo);
        funcionarioObtido.Cinema.Should().NotBeNull();
        funcionarioObtido.Cinema!.Id.Should().Be(cinema.Id);
    }

    [Fact]
    public void ObterFuncionarioComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var idInvalido = 999;

        // Act
        Action act = () => servico.ObterFuncionario(idInvalido);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage($"Funcionario com ID {idInvalido} nao encontrado.");
    }

    #endregion

    #region Testes ListarFuncionarios

    [Fact]
    public void ListarFuncionariosQuandoNaoHaFuncionarios()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);

        // Act
        var funcionarios = servico.ListarFuncionarios();

        // Assert
        funcionarios.Should().BeEmpty();
    }

    [Fact]
    public void ListarFuncionariosQuandoHaFuncionarios()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario1 = CriarFuncionarioPadrao(nome: "João", cargo: CargoFuncionario.AtendenteIngressos, cinema: cinema);
        var funcionario2 = CriarFuncionarioPadrao(nome: "Maria", cargo: CargoFuncionario.Gerente, cinema: cinema);
        var funcionario3 = CriarFuncionarioPadrao(nome: "Pedro", cargo: CargoFuncionario.Limpeza, cinema: cinema);

        servico.CriarFuncionario(funcionario1);
        servico.CriarFuncionario(funcionario2);
        servico.CriarFuncionario(funcionario3);

        // Act
        var funcionarios = servico.ListarFuncionarios();

        // Assert
        funcionarios.Should().HaveCount(3);
        funcionarios.Should().Contain(f => f.Nome == "João");
        funcionarios.Should().Contain(f => f.Nome == "Maria");
        funcionarios.Should().Contain(f => f.Nome == "Pedro");
    }

    [Fact]
    public void ListarFuncionariosIncluirCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao(nome: "Cine Star");

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(cinema: cinema);
        servico.CriarFuncionario(funcionario);

        // Act
        var funcionarios = servico.ListarFuncionarios();

        // Assert
        funcionarios.Should().HaveCount(1);
        funcionarios[0].Cinema.Should().NotBeNull();
        funcionarios[0].Cinema!.Nome.Should().Be("Cine Star");
    }

    #endregion

    #region Testes ListarPorCargo

    [Fact]
    public void ListarPorCargoQuandoNaoHaFuncionarios()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);

        // Act
        var funcionarios = servico.ListarPorCargo(CargoFuncionario.Gerente);

        // Assert
        funcionarios.Should().BeEmpty();
    }

    [Fact]
    public void ListarPorCargoQuandoHaFuncionariosDoCargo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var gerente1 = CriarFuncionarioPadrao(nome: "João", cargo: CargoFuncionario.Gerente, cinema: cinema);
        var gerente2 = CriarFuncionarioPadrao(nome: "Maria", cargo: CargoFuncionario.Gerente, cinema: cinema);
        var atendente = CriarFuncionarioPadrao(nome: "Pedro", cargo: CargoFuncionario.AtendenteIngressos, cinema: cinema);

        servico.CriarFuncionario(gerente1);
        servico.CriarFuncionario(gerente2);
        servico.CriarFuncionario(atendente);

        // Act
        var gerentes = servico.ListarPorCargo(CargoFuncionario.Gerente);

        // Assert
        gerentes.Should().HaveCount(2);
        gerentes.Should().OnlyContain(f => f.Cargo == CargoFuncionario.Gerente);
        gerentes.Should().Contain(f => f.Nome == "João");
        gerentes.Should().Contain(f => f.Nome == "Maria");
        gerentes.Should().NotContain(f => f.Nome == "Pedro");
    }

    [Fact]
    public void ListarPorCargoComDiferentesCargos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        servico.CriarFuncionario(CriarFuncionarioPadrao(nome: "Atendente1", cargo: CargoFuncionario.AtendenteIngressos, cinema: cinema));
        servico.CriarFuncionario(CriarFuncionarioPadrao(nome: "Atendente2", cargo: CargoFuncionario.AtendenteAlimentos, cinema: cinema));
        servico.CriarFuncionario(CriarFuncionarioPadrao(nome: "Limpeza1", cargo: CargoFuncionario.Limpeza, cinema: cinema));
        servico.CriarFuncionario(CriarFuncionarioPadrao(nome: "Garcom1", cargo: CargoFuncionario.Garcom, cinema: cinema));
        servico.CriarFuncionario(CriarFuncionarioPadrao(nome: "Gerente1", cargo: CargoFuncionario.Gerente, cinema: cinema));

        // Act & Assert
        servico.ListarPorCargo(CargoFuncionario.AtendenteIngressos).Should().HaveCount(1);
        servico.ListarPorCargo(CargoFuncionario.AtendenteAlimentos).Should().HaveCount(1);
        servico.ListarPorCargo(CargoFuncionario.Limpeza).Should().HaveCount(1);
        servico.ListarPorCargo(CargoFuncionario.Garcom).Should().HaveCount(1);
        servico.ListarPorCargo(CargoFuncionario.Gerente).Should().HaveCount(1);
    }

    #endregion

    #region Testes ListarPorCinema

    [Fact]
    public void ListarPorCinemaQuandoNaoHaFuncionarios()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);

        // Act
        var funcionarios = servico.ListarPorCinema(1);

        // Assert
        funcionarios.Should().BeEmpty();
    }

    [Fact]
    public void ListarPorCinemaQuandoHaFuncionariosDoCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema1 = CriarCinemaPadrao(id: 1, nome: "Cinema A");
        var cinema2 = CriarCinemaPadrao(id: 2, nome: "Cinema B");

        context.Cinemas.Add(cinema1);
        context.Cinemas.Add(cinema2);
        context.SaveChanges();

        var func1 = CriarFuncionarioPadrao(nome: "João", cinema: cinema1);
        var func2 = CriarFuncionarioPadrao(nome: "Maria", cinema: cinema1);
        var func3 = CriarFuncionarioPadrao(nome: "Pedro", cinema: cinema2);

        servico.CriarFuncionario(func1);
        servico.CriarFuncionario(func2);
        servico.CriarFuncionario(func3);

        // Act
        var funcionariosCinema1 = servico.ListarPorCinema(cinema1.Id);

        // Assert
        funcionariosCinema1.Should().HaveCount(2);
        funcionariosCinema1.Should().OnlyContain(f => f.Cinema!.Id == cinema1.Id);
        funcionariosCinema1.Should().Contain(f => f.Nome == "João");
        funcionariosCinema1.Should().Contain(f => f.Nome == "Maria");
        funcionariosCinema1.Should().NotContain(f => f.Nome == "Pedro");
    }

    [Fact]
    public void ListarPorCinemaComCinemaInexistente()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao(id: 1);

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(cinema: cinema);
        servico.CriarFuncionario(funcionario);

        // Act
        var funcionarios = servico.ListarPorCinema(999);

        // Assert
        funcionarios.Should().BeEmpty();
    }

    #endregion

    #region Testes AtualizarFuncionario

    [Fact]
    public void AtualizarFuncionarioComNovoNome()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(nome: "João Silva", cinema: cinema);
        servico.CriarFuncionario(funcionario);

        // Act
        servico.AtualizarFuncionario(funcionario.Id, nome: "João Santos");

        // Assert
        var funcionarioAtualizado = servico.ObterFuncionario(funcionario.Id);
        funcionarioAtualizado.Nome.Should().Be("João Santos");
        funcionarioAtualizado.DataAtualizacao.Should().NotBeNull();
        funcionarioAtualizado.DataAtualizacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AtualizarFuncionarioComNovoCargo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(cargo: CargoFuncionario.AtendenteIngressos, cinema: cinema);
        servico.CriarFuncionario(funcionario);

        // Act
        servico.AtualizarFuncionario(funcionario.Id, cargo: CargoFuncionario.Gerente);

        // Assert
        var funcionarioAtualizado = servico.ObterFuncionario(funcionario.Id);
        funcionarioAtualizado.Cargo.Should().Be(CargoFuncionario.Gerente);
        funcionarioAtualizado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarFuncionarioComNovoCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema1 = CriarCinemaPadrao(id: 1, nome: "Cinema A");
        var cinema2 = CriarCinemaPadrao(id: 2, nome: "Cinema B");

        context.Cinemas.Add(cinema1);
        context.Cinemas.Add(cinema2);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(cinema: cinema1);
        servico.CriarFuncionario(funcionario);

        // Act
        servico.AtualizarFuncionario(funcionario.Id, cinema: cinema2);

        // Assert
        var funcionarioAtualizado = servico.ObterFuncionario(funcionario.Id);
        funcionarioAtualizado.Cinema.Should().NotBeNull();
        funcionarioAtualizado.Cinema!.Id.Should().Be(cinema2.Id);
        funcionarioAtualizado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarFuncionarioComTodosOsParametros()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema1 = CriarCinemaPadrao(id: 1, nome: "Cinema A");
        var cinema2 = CriarCinemaPadrao(id: 2, nome: "Cinema B");

        context.Cinemas.Add(cinema1);
        context.Cinemas.Add(cinema2);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(nome: "João", cargo: CargoFuncionario.AtendenteIngressos, cinema: cinema1);
        servico.CriarFuncionario(funcionario);

        // Act
        servico.AtualizarFuncionario(funcionario.Id, nome: "Maria", cargo: CargoFuncionario.Gerente, cinema: cinema2);

        // Assert
        var funcionarioAtualizado = servico.ObterFuncionario(funcionario.Id);
        funcionarioAtualizado.Nome.Should().Be("Maria");
        funcionarioAtualizado.Cargo.Should().Be(CargoFuncionario.Gerente);
        funcionarioAtualizado.Cinema!.Id.Should().Be(cinema2.Id);
        funcionarioAtualizado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarFuncionarioSemParametros()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(nome: "João", cargo: CargoFuncionario.Limpeza, cinema: cinema);
        servico.CriarFuncionario(funcionario);

        var nomeOriginal = funcionario.Nome;
        var cargoOriginal = funcionario.Cargo;
        var cinemaOriginal = funcionario.Cinema;

        // Act
        servico.AtualizarFuncionario(funcionario.Id);

        // Assert
        var funcionarioAtualizado = servico.ObterFuncionario(funcionario.Id);
        funcionarioAtualizado.Nome.Should().Be(nomeOriginal);
        funcionarioAtualizado.Cargo.Should().Be(cargoOriginal);
        funcionarioAtualizado.Cinema!.Id.Should().Be(cinemaOriginal!.Id);
        funcionarioAtualizado.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarFuncionarioComNomeVazio()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(nome: "João", cinema: cinema);
        servico.CriarFuncionario(funcionario);

        // Act
        servico.AtualizarFuncionario(funcionario.Id, nome: "");

        // Assert
        var funcionarioAtualizado = servico.ObterFuncionario(funcionario.Id);
        funcionarioAtualizado.Nome.Should().Be("João");
    }

    [Fact]
    public void AtualizarFuncionarioComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var idInvalido = 999;

        // Act
        Action act = () => servico.AtualizarFuncionario(idInvalido, nome: "Novo Nome");

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage($"Funcionario com ID {idInvalido} nao encontrado.");
    }

    [Fact]
    public void AtualizarFuncionarioComMesmoCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(cinema: cinema);
        servico.CriarFuncionario(funcionario);

        var cinemaOriginal = funcionario.Cinema;

        // Act
        servico.AtualizarFuncionario(funcionario.Id, cinema: cinema);

        // Assert
        var funcionarioAtualizado = servico.ObterFuncionario(funcionario.Id);
        funcionarioAtualizado.Cinema!.Id.Should().Be(cinemaOriginal!.Id);
    }

    #endregion

    #region Testes DeletarFuncionario

    [Fact]
    public void DeletarFuncionarioComIdValido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var funcionario = CriarFuncionarioPadrao(cinema: cinema);
        servico.CriarFuncionario(funcionario);

        // Act
        servico.DeletarFuncionario(funcionario.Id);

        // Assert
        var funcionarioDeletado = context.Funcionarios.FirstOrDefault(f => f.Id == funcionario.Id);
        funcionarioDeletado.Should().BeNull();
    }

    [Fact]
    public void DeletarFuncionarioComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var idInvalido = 999;

        // Act
        Action act = () => servico.DeletarFuncionario(idInvalido);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage($"Funcionario com ID {idInvalido} nao encontrado.");
    }

    [Fact]
    public void DeletarFuncionarioAposDeletar()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new FuncionarioServico(context);
        var cinema = CriarCinemaPadrao();

        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var func1 = CriarFuncionarioPadrao(nome: "João", cinema: cinema);
        var func2 = CriarFuncionarioPadrao(nome: "Maria", cinema: cinema);

        servico.CriarFuncionario(func1);
        servico.CriarFuncionario(func2);

        // Act
        servico.DeletarFuncionario(func1.Id);
        var funcionarios = servico.ListarFuncionarios();

        // Assert
        funcionarios.Should().HaveCount(1);
        funcionarios.Should().Contain(f => f.Id == func2.Id);
        funcionarios.Should().NotContain(f => f.Id == func1.Id);
    }

    #endregion
}
