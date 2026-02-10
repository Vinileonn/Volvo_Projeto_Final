using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class IngressoServicoTests
{
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    private static Filme CriarFilmePadrao(int id = 1, ClassificacaoIndicativa classificacao = ClassificacaoIndicativa.Livre)
    {
        return new Filme
        {
            Id = id,
            Titulo = "Filme Teste",
            Duracao = 120,
            Genero = "Ação",
            AnoLancamento = new DateTime(2025, 1, 1),
            Eh3D = false,
            Classificacao = classificacao
        };
    }

    private static Sala CriarSalaPadrao(int id = 1, TipoSala tipo = TipoSala.Normal)
    {
        var sala = new Sala
        {
            Id = id,
            Nome = "Sala 1",
            Capacidade = 100,
            Tipo = tipo
        };

        sala.Assentos.Add(new Assento('A', 1, sala, tipo: TipoAssento.Normal));
        sala.Assentos.Add(new Assento('A', 2, sala, tipo: TipoAssento.Normal));
        
        return sala;
    }

    private static Sessao CriarSessaoPadrao(int id = 1, Filme? filme = null, Sala? sala = null, DateTime? dataHorario = null, decimal precoBase = 30m)
    {
        return new Sessao
        {
            Id = id,
            DataHorario = dataHorario ?? DateTime.Now.AddDays(2),
            PrecoBase = precoBase,
            PrecoFinal = precoBase,
            Tipo = TipoSessao.Regular,
            Idioma = IdiomaSessao.Dublado,
            Filme = filme ?? CriarFilmePadrao(),
            Sala = sala ?? CriarSalaPadrao()
        };
    }

    private static Cliente CriarClientePadrao(int id = 1, DateTime? dataNascimento = null, int pontosFidelidade = 0)
    {
        return new Cliente
        {
            Id = id,
            Nome = "Cliente Teste",
            Email = "cliente@teste.com",
            Senha = "senha123",
            CPF = "12345678901",
            Telefone = "1234567890",
            Endereco = "Rua Teste, 123",
            DataNascimento = dataNascimento ?? new DateTime(2000, 1, 1),
            PontosFidelidade = pontosFidelidade
        };
    }

    [Fact]
    public void VenderInteira_ComDadosValidos_DeveCriarComSucesso()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);

        ingresso.Should().NotBeNull();
        ingresso.Should().BeOfType<IngressoInteira>();
        ingresso.Fila.Should().Be('A');
        ingresso.Numero.Should().Be(1);
    }

    [Fact]
    public void VenderInteira_ComDadosInvalidos_DeveLancarExcecao()
    {
        var context = CriarContextoEmMemoria();
        var servico = new IngressoServico(context);
        var cliente = CriarClientePadrao();
        var sessao = CriarSessaoPadrao();

        Action actSessaoNula = () => servico.VenderInteira(null!, cliente, 'A', 1, FormaPagamento.Credito);
        actSessaoNula.Should().Throw<DadosInvalidosExcecao>();

        Action actClienteNulo = () => servico.VenderInteira(sessao, null!, 'A', 1, FormaPagamento.Credito);
        actClienteNulo.Should().Throw<DadosInvalidosExcecao>();
    }

    [Fact]
    public void VenderInteira_ClienteMenorDeIdade_DeveLancarExcecao()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao(classificacao: ClassificacaoIndicativa.Dezoito);
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        var cliente = CriarClientePadrao(dataNascimento: DateTime.Now.AddYears(-16));

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);

        Action act = () => servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);
        act.Should().Throw<OperacaoNaoPermitidaExcecao>();
    }

    [Fact]
    public void VenderInteira_ComAssentoJaOcupado_DeveLancarExcecao()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        var cliente1 = CriarClientePadrao(id: 1);
        var cliente2 = CriarClientePadrao(id: 2);

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente1);
        context.Clientes.Add(cliente2);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        servico.VenderInteira(sessao, cliente1, 'A', 1, FormaPagamento.Credito);

        Action act = () => servico.VenderInteira(sessao, cliente2, 'A', 1, FormaPagamento.Credito);
        act.Should().Throw<OperacaoNaoPermitidaExcecao>();
    }

    [Fact]
    public void VenderInteira_ComPagamentoDinheiro_DeveCalcularTroco()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, precoBase: 30m);
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Dinheiro, valorPago: 50m);

        ingresso.ValorPago.Should().Be(50m);
        ingresso.ValorTroco.Should().Be(20m);
    }

    [Fact]
    public void VenderInteira_ComPagamentoDinheiroInsuficiente_DeveLancarExcecao()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, precoBase: 30m);
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);

        Action act = () => servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Dinheiro, valorPago: 20m);
        act.Should().Throw<DadosInvalidosExcecao>();
    }

    [Fact]
    public void VenderInteira_ComReservaAntecipada_DeveAdicionarTaxa()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, dataHorario: DateTime.Now.AddHours(2), precoBase: 30m);
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito, reservaAntecipada: true);

        ingresso.ReservaAntecipada.Should().BeTrue();
        ingresso.TaxaReserva.Should().Be(5m);
    }

    [Fact]
    public void VenderMeia_ComDadosValidos_DeveCriarComSucesso()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderMeia(sessao, cliente, 'A', 1, "Estudante", FormaPagamento.Credito);

        ingresso.Should().NotBeNull();
        ingresso.Should().BeOfType<IngressoMeia>();
        var ingressoMeia = ingresso as IngressoMeia;
        ingressoMeia!.Motivo.Should().Be("Estudante");
    }

    [Fact]
    public void VenderMeia_SemMotivo_DeveLancarExcecao()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        var cliente = CriarClientePadrao();

        var servico = new IngressoServico(context);

        Action act = () => servico.VenderMeia(sessao, cliente, 'A', 1, "", FormaPagamento.Credito);
        act.Should().Throw<DadosInvalidosExcecao>();
    }

    [Fact]
    public void ObterIngresso_ComIdValido_DeveRetornarIngresso()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingressoVendido = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);

        var ingresso = servico.ObterIngresso(ingressoVendido.Id);

        ingresso.Should().NotBeNull();
        ingresso.Id.Should().Be(ingressoVendido.Id);
    }

    [Fact]
    public void ObterIngresso_ComIdInvalido_DeveLancarExcecao()
    {
        var context = CriarContextoEmMemoria();
        var servico = new IngressoServico(context);

        Action act = () => servico.ObterIngresso(999);
        act.Should().Throw<RecursoNaoEncontradoExcecao>();
    }

    [Fact]
    public void ListarIngressos_DeveRetornarTodos()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);
        servico.VenderMeia(sessao, cliente, 'A', 2, "Estudante", FormaPagamento.Credito);

        var ingressos = servico.ListarIngressos();

        ingressos.Should().HaveCount(2);
        ingressos.Should().Contain(i => i is IngressoInteira);
        ingressos.Should().Contain(i => i is IngressoMeia);
    }

    [Fact]
    public void CancelarIngresso_ComAntecedencia_DeveCancelarComSucesso()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, dataHorario: DateTime.Now.AddDays(2));
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);

        servico.CancelarIngresso(ingresso.Id);

        var ingressoCancelado = context.Ingressos.FirstOrDefault(i => i.Id == ingresso.Id);
        ingressoCancelado.Should().BeNull();
    }

    [Fact]
    public void CancelarIngresso_SemAntecedencia_DeveLancarExcecao()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, dataHorario: DateTime.Now.AddHours(12));
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);

        Action act = () => servico.CancelarIngresso(ingresso.Id);
        act.Should().Throw<OperacaoNaoPermitidaExcecao>();
    }

    [Fact]
    public void CancelarIngresso_DeveLiberarAssento()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, dataHorario: DateTime.Now.AddDays(2));
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);
        var assento = sala.ConsultarAssento('A', 1);

        servico.CancelarIngresso(ingresso.Id);

        assento!.Disponivel.Should().BeTrue();
        assento.Ingresso.Should().BeNull();
    }

    [Fact]
    public void RealizarCheckIn_ComIngressoValido_DeveRealizarComSucesso()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, dataHorario: DateTime.Now.AddHours(2));
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);

        servico.RealizarCheckIn(ingresso.Id);

        var ingressoAtualizado = context.Ingressos.FirstOrDefault(i => i.Id == ingresso.Id);
        ingressoAtualizado!.CheckInRealizado.Should().BeTrue();
        ingressoAtualizado.DataCheckIn.Should().NotBeNull();
    }

    [Fact]
    public void RealizarCheckIn_ComCheckInJaRealizado_DeveLancarExcecao()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, dataHorario: DateTime.Now.AddHours(2));
        var cliente = CriarClientePadrao();

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);
        servico.RealizarCheckIn(ingresso.Id);

        Action act = () => servico.RealizarCheckIn(ingresso.Id);
        act.Should().Throw<OperacaoNaoPermitidaExcecao>();
    }

    [Fact]
    public void RealizarCheckIn_DeveAdicionarPontosFidelidade()
    {
        var context = CriarContextoEmMemoria();
        var filme = CriarFilmePadrao();
        var sala = CriarSalaPadrao();
        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, dataHorario: DateTime.Now.AddHours(2));
        var cliente = CriarClientePadrao(pontosFidelidade: 0);

        context.Filmes.Add(filme);
        context.Salas.Add(sala);
        context.Sessoes.Add(sessao);
        context.Clientes.Add(cliente);
        context.SaveChanges();

        var servico = new IngressoServico(context);
        var ingresso = servico.VenderInteira(sessao, cliente, 'A', 1, FormaPagamento.Credito);
        var pontosAntes = cliente.PontosFidelidade;

        servico.RealizarCheckIn(ingresso.Id);

        var clienteAtualizado = context.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
        clienteAtualizado!.PontosFidelidade.Should().BeGreaterThan(pontosAntes);
    }
}
