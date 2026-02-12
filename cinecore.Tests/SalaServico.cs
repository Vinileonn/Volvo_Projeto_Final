using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class SalaServicoTests
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

    private static Sala CriarSalaPadrao(
        int id = 1,
        Cinema? cinema = null,
        TipoSala tipo = TipoSala.Normal,
        string? nome = null,
        int capacidade = 100,
        int quantidadeAssentosCasal = 0,
        int quantidadeAssentosPCD = 0)
    {
        return new Sala
        {
            Id = id,
            Nome = nome ?? "Sala 1",
            Capacidade = capacidade,
            Cinema = cinema,
            Tipo = tipo,
            QuantidadeAssentosCasal = quantidadeAssentosCasal,
            QuantidadeAssentosPCD = quantidadeAssentosPCD
        };
    }

    #region Testes CriarSala

    [Fact]
    public void CriarSalaComDadosValidos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema);

        // Act
        servico.CriarSala(sala);

        // Assert
        var salaDb = context.Salas.Include(s => s.Assentos).FirstOrDefault(s => s.Id == sala.Id);
        salaDb.Should().NotBeNull();
        salaDb!.Nome.Should().Be("Sala 1");
        salaDb.Capacidade.Should().Be(100);
        salaDb.Assentos.Should().HaveCount(100);
    }

    [Fact]
    public void CriarSalaComSalaNula()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);

        // Act
        Action act = () => servico.CriarSala(null!);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Sala nao pode ser nula.");
    }

    [Fact]
    public void CriarSalaComNomeVazio()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var sala = CriarSalaPadrao(nome: "");

        // Act
        Action act = () => servico.CriarSala(sala);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Nome da sala e obrigatorio.");
    }

    [Fact]
    public void CriarSalaComCapacidadeZero()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var sala = CriarSalaPadrao(capacidade: 0);

        // Act
        Action act = () => servico.CriarSala(sala);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Capacidade deve ser maior que zero.");
    }

    [Fact]
    public void CriarSalaComCapacidadeNegativa()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var sala = CriarSalaPadrao(capacidade: -10);

        // Act
        Action act = () => servico.CriarSala(sala);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Capacidade deve ser maior que zero.");
    }

    [Fact]
    public void CriarSalaComAssentosCasalNegativo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var sala = CriarSalaPadrao(quantidadeAssentosCasal: -5);

        // Act
        Action act = () => servico.CriarSala(sala);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade de assentos especiais invalida.");
    }

    [Fact]
    public void CriarSalaComAssentosPCDNegativo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var sala = CriarSalaPadrao(quantidadeAssentosPCD: -5);

        // Act
        Action act = () => servico.CriarSala(sala);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade de assentos especiais invalida.");
    }

    [Fact]
    public void CriarSalaComAssentosEspeciaisExcedendoCapacidade()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var sala = CriarSalaPadrao(
            capacidade: 50,
            quantidadeAssentosCasal: 20, // 20 * 2 = 40 lugares
            quantidadeAssentosPCD: 15     // 40 + 15 = 55 > 50
        );

        // Act
        Action act = () => servico.CriarSala(sala);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade de assentos especiais excede a capacidade.");
    }

    [Fact]
    public void CriarSalaComNomeDuplicadoNoMesmoCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala1 = CriarSalaPadrao(id: 1, cinema: cinema, nome: "Sala A");
        servico.CriarSala(sala1);

        var sala2 = CriarSalaPadrao(id: 2, cinema: cinema, nome: "Sala A");

        // Act
        Action act = () => servico.CriarSala(sala2);

        // Assert
        act.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("Sala 'Sala A' ja existe neste cinema.");
    }

    [Fact]
    public void CriarSalaComAssentosEspeciais()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(
            cinema: cinema,
            capacidade: 30,
            quantidadeAssentosCasal: 5,
            quantidadeAssentosPCD: 5
        );

        // Act
        servico.CriarSala(sala);

        // Assert
        var salaDb = context.Salas.Include(s => s.Assentos).FirstOrDefault(s => s.Id == sala.Id);
        salaDb.Should().NotBeNull();
        // 5 assentos casal (ocupam 10 lugares) + 5 PCD (ocupam 5 lugares) + 15 normais = 25 assentos físicos
        // Mas ocupam os 30 lugares de capacidade
        salaDb!.Assentos.Count(a => a.Tipo == TipoAssento.Casal).Should().Be(5);
        salaDb.Assentos.Count(a => a.Tipo == TipoAssento.PCD).Should().Be(5);
        salaDb.Assentos.Count(a => a.Tipo == TipoAssento.Normal).Should().BeGreaterThan(0);
    }

    #endregion

    #region Testes ObterSala

    [Fact]
    public void ObterSalaComIdValido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        
        var sala = CriarSalaPadrao(cinema: cinema);
        servico.CriarSala(sala);

        // Act
        var resultado = servico.ObterSala(sala.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(sala.Id);
        resultado.Nome.Should().Be("Sala 1");
        resultado.Cinema.Should().NotBeNull();
    }

    [Fact]
    public void ObterSalaComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);

        // Act
        Action act = () => servico.ObterSala(999);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Sala com ID 999 não encontrada.");
    }

    #endregion

    #region Testes ListarSalas

    [Fact]
    public void ListarSalasComSalasCadastradas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala1 = CriarSalaPadrao(id: 1, cinema: cinema, nome: "Sala 1");
        var sala2 = CriarSalaPadrao(id: 2, cinema: cinema, nome: "Sala 2");
        var sala3 = CriarSalaPadrao(id: 3, cinema: cinema, nome: "Sala 3");
        
        servico.CriarSala(sala1);
        servico.CriarSala(sala2);
        servico.CriarSala(sala3);

        // Act
        var lista = servico.ListarSalas();

        // Assert
        lista.Should().HaveCount(3);
        lista.Select(s => s.Nome).Should().Contain(new[] { "Sala 1", "Sala 2", "Sala 3" });
    }

    [Fact]
    public void ListarSalasSemSalasCadastradas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);

        // Act
        var lista = servico.ListarSalas();

        // Assert
        lista.Should().BeEmpty();
    }

    #endregion

    #region Testes ListarSalasPorCinema

    [Fact]
    public void ListarSalasPorCinemaComSalasNoCinema()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        
        var cinema1 = CriarCinemaPadrao(id: 1, nome: "Cinema 1");
        var cinema2 = CriarCinemaPadrao(id: 2, nome: "Cinema 2");
        context.Cinemas.AddRange(cinema1, cinema2);
        context.SaveChanges();

        var sala1 = CriarSalaPadrao(id: 1, cinema: cinema1, nome: "Sala A");
        var sala2 = CriarSalaPadrao(id: 2, cinema: cinema1, nome: "Sala B");
        var sala3 = CriarSalaPadrao(id: 3, cinema: cinema2, nome: "Sala C");
        
        servico.CriarSala(sala1);
        servico.CriarSala(sala2);
        servico.CriarSala(sala3);

        // Act
        var lista = servico.ListarSalasPorCinema(cinema1.Id);

        // Assert
        lista.Should().HaveCount(2);
        lista.Select(s => s.Nome).Should().Contain(new[] { "Sala A", "Sala B" });
        lista.Should().NotContain(s => s.Nome == "Sala C");
    }

    [Fact]
    public void ListarSalasPorCinemaComCinemaSemSalas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        // Act
        var lista = servico.ListarSalasPorCinema(cinema.Id);

        // Assert
        lista.Should().BeEmpty();
    }

    #endregion

    #region Testes AtualizarSala

    [Fact]
    public void AtualizarSalaComNomeValido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema, nome: "Sala Original");
        servico.CriarSala(sala);

        // Act
        servico.AtualizarSala(sala.Id, nome: "Sala Atualizada");

        // Assert
        var salaDb = context.Salas.Find(sala.Id);
        salaDb.Should().NotBeNull();
        salaDb!.Nome.Should().Be("Sala Atualizada");
        salaDb.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void AtualizarSalaComCapacidadeValida()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema, capacidade: 100);
        servico.CriarSala(sala);

        // Act
        servico.AtualizarSala(sala.Id, capacidade: 150);

        // Assert
        var salaDb = context.Salas.Include(s => s.Assentos).FirstOrDefault(s => s.Id == sala.Id);
        salaDb.Should().NotBeNull();
        salaDb!.Capacidade.Should().Be(150);
        salaDb.Assentos.Should().HaveCount(150); // Deve regenerar assentos
    }

    [Fact]
    public void AtualizarSalaComCapacidadeZero()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema);
        servico.CriarSala(sala);

        // Act
        Action act = () => servico.AtualizarSala(sala.Id, capacidade: 0);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Capacidade deve ser maior que zero.");
    }

    [Fact]
    public void AtualizarSalaComNomeDuplicado()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala1 = CriarSalaPadrao(id: 1, cinema: cinema, nome: "Sala A");
        var sala2 = CriarSalaPadrao(id: 2, cinema: cinema, nome: "Sala B");
        servico.CriarSala(sala1);
        servico.CriarSala(sala2);

        // Act
        Action act = () => servico.AtualizarSala(sala2.Id, nome: "Sala A");

        // Assert
        act.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("Ja existe uma sala com o nome 'Sala A' neste cinema.");
    }

    [Fact]
    public void AtualizarSalaComAssentosCasalNegativo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema);
        servico.CriarSala(sala);

        // Act
        Action act = () => servico.AtualizarSala(sala.Id, quantidadeAssentosCasal: -5);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade de assentos casal invalida.");
    }

    [Fact]
    public void AtualizarSalaComAssentosPCDNegativo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema);
        servico.CriarSala(sala);

        // Act
        Action act = () => servico.AtualizarSala(sala.Id, quantidadeAssentosPCD: -5);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade de assentos PCD invalida.");
    }

    [Fact]
    public void AtualizarSalaComAssentosEspeciaisExcedendoCapacidade()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema, capacidade: 50);
        servico.CriarSala(sala);

        // Act
        Action act = () => servico.AtualizarSala(
            sala.Id,
            quantidadeAssentosCasal: 20,
            quantidadeAssentosPCD: 15
        );

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade de assentos especiais excede a capacidade.");
    }

    [Fact]
    public void AtualizarSalaComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);

        // Act
        Action act = () => servico.AtualizarSala(999, nome: "Nova Sala");

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Sala com ID 999 não encontrada.");
    }

    #endregion

    #region Testes DeletarSala

    [Fact]
    public void DeletarSalaComIdValido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema);
        servico.CriarSala(sala);

        // Act
        servico.DeletarSala(sala.Id);

        // Assert
        var salaDb = context.Salas.FirstOrDefault(s => s.Id == sala.Id);
        salaDb.Should().BeNull();
    }

    [Fact]
    public void DeletarSalaComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);

        // Act
        Action act = () => servico.DeletarSala(999);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Sala com ID 999 não encontrada.");
    }

    #endregion

    #region Testes VisualizarSala

    [Fact]
    public void VisualizarSalaComSalaValida()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);
        var cinema = CriarCinemaPadrao();
        context.Cinemas.Add(cinema);
        context.SaveChanges();

        var sala = CriarSalaPadrao(cinema: cinema, capacidade: 20);
        servico.CriarSala(sala);

        // Act
        var visualizacao = servico.VisualizarSala(sala.Id);

        // Assert
        visualizacao.Should().NotBeNull();
        visualizacao.Should().NotBeEmpty();
        visualizacao.Values.SelectMany(v => v).Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void VisualizarSalaComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new SalaServico(context);

        // Act
        Action act = () => servico.VisualizarSala(999);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Sala com ID 999 não encontrada.");
    }

    #endregion
}
