using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class AluguelSalaServicoTests
{
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    private static Cinema CriarCinemaPadrao(int id = 1)
    {
        return new Cinema { Id = id, Nome = "Cinema Teste", Endereco = "Rua Teste, 123" };
    }

    private static Sala CriarSalaPadrao(int id = 1, TipoSala tipo = TipoSala.Normal, Cinema? cinema = null)
    {
        return new Sala { Id = id, Nome = "Sala Teste", Capacidade = 50, Tipo = tipo, Cinema = cinema ?? CriarCinemaPadrao() };
    }

    private static AluguelSala CriarAluguelSalaPadrao(int id = 0, Sala? sala = null, string? nomeCliente = null, DateTime? inicio = null, decimal valor = 0m)
    {
        var agora = DateTime.Now;
        return new AluguelSala
        {
            Id = id,
            NomeCliente = nomeCliente ?? "Cliente Teste",
            Contato = "(11) 99999-9999",
            Sala = sala ?? CriarSalaPadrao(),
            Inicio = inicio ?? agora.AddDays(1),
            Fim = agora.AddDays(1).AddHours(3),
            Motivo = "Evento teste",
            Valor = valor,
            Status = StatusAluguel.Solicitado
        };
    }

    [Fact]
    public void SolicitarAluguel_ComDadosValidos() // DeveCriarComSucesso
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var aluguel = CriarAluguelSalaPadrao(sala: sala, valor: 600m);
        
        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        servico.SolicitarAluguel(aluguel);

        var aluguelDb = context.AlugueisSala.FirstOrDefault(a => a.Id == aluguel.Id);
        aluguelDb.Should().NotBeNull();
        aluguelDb!.Status.Should().Be(StatusAluguel.Solicitado);
    }

    [Fact]
    public void SolicitarAluguel_ComDadosInvalidos() // DeveLancarExcecao
    {
        var context = CriarContextoEmMemoria();
        var servico = new AluguelSalaServico(context, new SessaoServico(context));

        Action actNulo = () => servico.SolicitarAluguel(null!);
        actNulo.Should().Throw<DadosInvalidosExcecao>();

        var aluguelSemCliente = CriarAluguelSalaPadrao(nomeCliente: "");
        Action actSemCliente = () => servico.SolicitarAluguel(aluguelSemCliente);
        actSemCliente.Should().Throw<DadosInvalidosExcecao>();
    }

    [Fact]
    public void SolicitarAluguel_ComPeriodoInvalido() // DeveLancarExcecao
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var agora = DateTime.Now;
        
        var aluguel = new AluguelSala
        {
            NomeCliente = "Cliente",
            Contato = "(11) 99999-9999",
            Sala = sala,
            Inicio = agora.AddDays(1),
            Fim = agora.AddDays(1).AddMinutes(-30),
            Motivo = "Evento teste",
            Valor = 600m
        };
        
        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));

        Action act = () => servico.SolicitarAluguel(aluguel);
        act.Should().Throw<DadosInvalidosExcecao>();
    }

    [Fact]
    public void SolicitarAluguel_ComConflitoAluguel() // DeveLancarExcecao
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var agora = DateTime.Now;
        
        var aluguel1 = CriarAluguelSalaPadrao(sala: sala, valor: 600m);
        var aluguel2 = CriarAluguelSalaPadrao(sala: sala, inicio: agora.AddDays(1).AddHours(2), valor: 600m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        servico.SolicitarAluguel(aluguel1);

        Action act = () => servico.SolicitarAluguel(aluguel2);
        act.Should().Throw<OperacaoNaoPermitidaExcecao>();
    }

    [Fact]
    public void SolicitarAluguel_SemValor() // DeveCalcularAutomaticamente
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var aluguel = CriarAluguelSalaPadrao(sala: sala, valor: 0m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        servico.SolicitarAluguel(aluguel);

        var aluguelDb = context.AlugueisSala.First();
        aluguelDb.Valor.Should().Be(600m);
    }

    [Fact]
    public void ObterAluguel_ComIdValido() // DeveRetornarAluguel
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var aluguel = CriarAluguelSalaPadrao(id: 1, sala: sala, valor: 600m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.AlugueisSala.Add(aluguel);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        var resultado = servico.ObterAluguel(1);
        
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
    }

    [Fact]
    public void ListarAlugueis() // DeveRetornarTodos
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);

        var aluguel1 = CriarAluguelSalaPadrao(id: 1, sala: sala, valor: 600m);
        var aluguel2 = CriarAluguelSalaPadrao(id: 2, sala: sala, valor: 800m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.AlugueisSala.AddRange(aluguel1, aluguel2);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        var resultado = servico.ListarAlugueis();
        
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public void ListarPorStatus() // DeveRetornarFiltrados
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);

        var aluguel1 = CriarAluguelSalaPadrao(id: 1, sala: sala, valor: 600m);
        aluguel1.Status = StatusAluguel.Aprovado;

        var aluguel2 = CriarAluguelSalaPadrao(id: 2, sala: sala, valor: 800m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.AlugueisSala.AddRange(aluguel1, aluguel2);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        var resultado = servico.ListarPorStatus(StatusAluguel.Aprovado);
        
        resultado.Should().HaveCount(1);
        resultado.First().Status.Should().Be(StatusAluguel.Aprovado);
    }

    [Fact]
    public void AprovarAluguel() // DeveAprovarComSucesso
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var aluguel = CriarAluguelSalaPadrao(id: 1, sala: sala, valor: 600m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.AlugueisSala.Add(aluguel);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        servico.AprovarAluguel(1, 800m);

        var aluguelDb = context.AlugueisSala.First(a => a.Id == 1);
        aluguelDb.Status.Should().Be(StatusAluguel.Aprovado);
        aluguelDb.Valor.Should().Be(800m);
    }

    [Fact]
    public void CancelarAluguel_ComAntecedencia() // DeveCancelar
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var aluguel = CriarAluguelSalaPadrao(id: 1, sala: sala, inicio: DateTime.Now.AddDays(2), valor: 600m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.AlugueisSala.Add(aluguel);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        servico.CancelarAluguel(1);

        var aluguelDb = context.AlugueisSala.First(a => a.Id == 1);
        aluguelDb.Status.Should().Be(StatusAluguel.Cancelado);
    }

    [Fact]
    public void CancelarAluguel_SemAntecedencia() // DeveLancarExcecao
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var aluguel = CriarAluguelSalaPadrao(id: 1, sala: sala, inicio: DateTime.Now.AddHours(12), valor: 600m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.AlugueisSala.Add(aluguel);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));

        Action act = () => servico.CancelarAluguel(1);
        act.Should().Throw<OperacaoNaoPermitidaExcecao>();
    }

    [Fact]
    public void AtualizarAluguel() // DeveAtualizarDados
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var aluguel = CriarAluguelSalaPadrao(id: 1, sala: sala, valor: 600m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.AlugueisSala.Add(aluguel);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        servico.AtualizarAluguel(1, nomeCliente: "Novo Nome");

        var aluguelDb = context.AlugueisSala.First(a => a.Id == 1);
        aluguelDb.NomeCliente.Should().Be("Novo Nome");
    }

    [Fact]
    public void DeletarAluguel_DeveRemover()
    {
        var context = CriarContextoEmMemoria();
        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var aluguel = CriarAluguelSalaPadrao(id: 1, sala: sala, valor: 600m);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.AlugueisSala.Add(aluguel);
        context.SaveChanges();

        var servico = new AluguelSalaServico(context, new SessaoServico(context));
        servico.DeletarAluguel(1);

        var aluguelDb = context.AlugueisSala.FirstOrDefault(a => a.Id == 1);
        aluguelDb.Should().BeNull();
    }
}
