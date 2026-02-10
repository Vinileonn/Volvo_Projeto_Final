using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class SessaoServicoTests
{
    // Helper para criar um contexto de banco de dados em memória
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    private static Filme CriarFilmePadrao(
        int id = 1,
        string? titulo = null,
        int duracao = 120,
        bool eh3D = false)
    {
        return new Filme
        {
            Id = id,
            Titulo = titulo ?? "Filme Teste",
            Duracao = duracao,
            Genero = "Ação",
            AnoLancamento = new DateTime(2025, 1, 1),
            Eh3D = eh3D,
            Classificacao = ClassificacaoIndicativa.Livre
        };
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
        string? nome = null)
    {
        return new Sala
        {
            Id = id,
            Nome = nome ?? "Sala 1",
            Capacidade = 100,
            Cinema = cinema,
            Tipo = tipo,
            QuantidadeAssentosCasal = 0,
            QuantidadeAssentosPCD = 0
        };
    }

    private static Sessao CriarSessaoPadrao(
        int id = 1,
        Filme? filme = null,
        Sala? sala = null,
        DateTime? dataHorario = null,
        decimal precoBase = 50m,
        TipoSessao tipo = TipoSessao.Regular,
        IdiomaSessao idioma = IdiomaSessao.Dublado)
    {
        return new Sessao
        {
            Id = id,
            DataHorario = dataHorario ?? new DateTime(2025, 3, 15, 19, 0, 0),
            PrecoBase = precoBase,
            PrecoFinal = precoBase,
            Tipo = tipo,
            Idioma = idioma,
            Filme = filme ?? CriarFilmePadrao(),
            Sala = sala ?? CriarSalaPadrao()
        };
    }

    private static Funcionario CriarFuncionarioPadrao(
        int id = 1,
        string? nome = null,
        CargoFuncionario cargo = CargoFuncionario.AtendenteIngressos,
        Cinema? cinema = null)
    {
        return new Funcionario
        {
            Id = id,
            Nome = nome ?? "Funcionário Teste",
            Cargo = cargo,
            Cinema = cinema
        };
    }

    // ==================== Testes de Criação ====================

    [Fact]
    public void CriarSessao_ComDadosValidos_DeveAdicionarNoContexto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);

        // Act
        sessaoServico.CriarSessao(sessao);

        // Assert
        var sessaoCadastrada = context.Sessoes.FirstOrDefault(s => s.DataHorario == sessao.DataHorario);
        sessaoCadastrada.Should().NotBeNull();
        sessaoCadastrada!.PrecoBase.Should().Be(50m);
        sessaoCadastrada.Tipo.Should().Be(TipoSessao.Regular);
    }

    [Fact]
    public void CriarSessao_ComSessaoNula_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        // Act & Assert
        var acao = () => sessaoServico.CriarSessao(null!);
        
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*Sessao nao pode ser nula*");
    }

    [Fact]
    public void CriarSessao_ComDataHorarioInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(
            filme: filme,
            sala: sala,
            dataHorario: default(DateTime));

        // Act & Assert
        var acao = () => sessaoServico.CriarSessao(sessao);
        
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*Data e horario sao obrigatorios*");
    }

    [Fact]
    public void CriarSessao_ComPrecoBaseNegativo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(
            filme: filme,
            sala: sala,
            precoBase: -10m);

        // Act & Assert
        var acao = () => sessaoServico.CriarSessao(sessao);
        
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*Preco base nao pode ser negativo*");
    }

    [Fact]
    public void CriarSessao_SemFilme_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.SaveChanges();

        var sessao = new Sessao
        {
            DataHorario = new DateTime(2025, 3, 15, 19, 0, 0),
            PrecoBase = 50m,
            PrecoFinal = 50m,
            Tipo = TipoSessao.Regular,
            Idioma = IdiomaSessao.Dublado,
            Filme = null,
            Sala = sala
        };

        // Act & Assert
        var acao = () => sessaoServico.CriarSessao(sessao);
        
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*Filme e obrigatorio*");
    }

    [Fact]
    public void CriarSessao_SemSala_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = new Sessao
        {
            DataHorario = new DateTime(2025, 3, 15, 19, 0, 0),
            PrecoBase = 50m,
            PrecoFinal = 50m,
            Tipo = TipoSessao.Regular,
            Idioma = IdiomaSessao.Dublado,
            Filme = filme,
            Sala = null
        };

        // Act & Assert
        var acao = () => sessaoServico.CriarSessao(sessao);
        
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*Sala e obrigatoria*");
    }

    [Fact]
    public void CriarSessao_ComConflitodeHorario_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao(duracao: 120);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var dataHorario = new DateTime(2025, 3, 15, 19, 0, 0);
        var sessao1 = CriarSessaoPadrao(
            id: 1,
            filme: filme,
            sala: sala,
            dataHorario: dataHorario);

        sessaoServico.CriarSessao(sessao1);

        // Uma segunda sessão no mesmo horário e sala
        var sessao2 = CriarSessaoPadrao(
            id: 2,
            filme: filme,
            sala: sala,
            dataHorario: dataHorario.AddMinutes(60)); // Horário conflitante

        // Act & Assert
        var acao = () => sessaoServico.CriarSessao(sessao2);
        
        acao.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("*Conflito de horario*");
    }

    [Fact]
    public void CriarSessao_TipoEvento_ComNomeEventoEParceiro_DevePreservarDados()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(
            filme: filme,
            sala: sala,
            tipo: TipoSessao.Evento);
        sessao.NomeEvento = "Evento Especial";
        sessao.Parceiro = "Parceiro X";

        // Act
        sessaoServico.CriarSessao(sessao);

        // Assert
        var sessaoCadastrada = context.Sessoes
            .Include(s => s.Filme)
            .FirstOrDefault(s => s.DataHorario == sessao.DataHorario);
        
        sessaoCadastrada.Should().NotBeNull();
        sessaoCadastrada!.NomeEvento.Should().Be("Evento Especial");
        sessaoCadastrada.Parceiro.Should().Be("Parceiro X");
    }

    [Fact]
    public void CriarSessao_TipoRegular_DeveNularNomeEventoEParceiro()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(
            filme: filme,
            sala: sala,
            tipo: TipoSessao.Regular);
        sessao.NomeEvento = "Evento Especial";
        sessao.Parceiro = "Parceiro X";

        // Act
        sessaoServico.CriarSessao(sessao);

        // Assert
        var sessaoCadastrada = context.Sessoes
            .Include(s => s.Filme)
            .FirstOrDefault(s => s.DataHorario == sessao.DataHorario);
        
        sessaoCadastrada.Should().NotBeNull();
        sessaoCadastrada!.NomeEvento.Should().BeNull();
        sessaoCadastrada.Parceiro.Should().BeNull();
    }

    // ==================== Testes de Leitura ====================

    [Fact]
    public void ObterSessao_ComIdExistente_DeveRetornarSessao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        sessaoServico.CriarSessao(sessao);

        var sessaoCriada = context.Sessoes.First();
        int idSessao = sessaoCriada.Id;

        // Act
        var sessaoObtida = sessaoServico.ObterSessao(idSessao);

        // Assert
        sessaoObtida.Should().NotBeNull();
        sessaoObtida.Id.Should().Be(idSessao);
        sessaoObtida.PrecoBase.Should().Be(50m);
    }

    [Fact]
    public void ObterSessao_ComIdInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        // Act & Assert
        var acao = () => sessaoServico.ObterSessao(999);
        
        acao.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("*Sessão com ID 999 não encontrada*");
    }

    [Fact]
    public void ListarSessoes_ComVariasSessoes_DeveRetornarTodas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala1 = CriarSalaPadrao(id: 1, cinema: cinema);
        var sala2 = CriarSalaPadrao(id: 2, cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala1);
        context.Salas.Add(sala2);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao1 = CriarSessaoPadrao(id: 1, filme: filme, sala: sala1);
        var sessao2 = CriarSessaoPadrao(
            id: 2,
            filme: filme,
            sala: sala2,
            dataHorario: new DateTime(2025, 3, 16, 19, 0, 0));

        sessaoServico.CriarSessao(sessao1);
        sessaoServico.CriarSessao(sessao2);

        // Act
        var sessoes = sessaoServico.ListarSessoes();

        // Assert
        sessoes.Should().HaveCount(2);
        sessoes.Should().AllSatisfy(s => s.Filme.Should().NotBeNull());
    }

    [Fact]
    public void ListarSessoes_SemSessoes_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        // Act
        var sessoes = sessaoServico.ListarSessoes();

        // Assert
        sessoes.Should().BeEmpty();
    }

    [Fact]
    public void ListarSessoesPorFilme_ComFilmeValido_DeveRetornarSessoes()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme1 = CriarFilmePadrao(id: 1, titulo: "Filme 1");
        var filme2 = CriarFilmePadrao(id: 2, titulo: "Filme 2");

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme1);
        context.Filmes.Add(filme2);
        context.SaveChanges();

        var sessao1 = CriarSessaoPadrao(id: 1, filme: filme1, sala: sala);
        var sessao2 = CriarSessaoPadrao(
            id: 2,
            filme: filme1,
            sala: sala,
            dataHorario: new DateTime(2025, 3, 16, 19, 0, 0));
        var sessao3 = CriarSessaoPadrao(
            id: 3,
            filme: filme2,
            sala: sala,
            dataHorario: new DateTime(2025, 3, 17, 19, 0, 0));

        sessaoServico.CriarSessao(sessao1);
        sessaoServico.CriarSessao(sessao2);
        sessaoServico.CriarSessao(sessao3);

        // Act
        var sessoes = sessaoServico.ListarSessoesPorFilme(filme1.Id);

        // Assert
        sessoes.Should().HaveCount(2);
        sessoes.Should().AllSatisfy(s => s.Filme!.Id.Should().Be(filme1.Id));
    }

    [Fact]
    public void ListarSessoesPorSala_ComSalaValida_DeveRetornarSessoes()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala1 = CriarSalaPadrao(id: 1, cinema: cinema);
        var sala2 = CriarSalaPadrao(id: 2, cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala1);
        context.Salas.Add(sala2);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao1 = CriarSessaoPadrao(id: 1, filme: filme, sala: sala1);
        var sessao2 = CriarSessaoPadrao(
            id: 2,
            filme: filme,
            sala: sala1,
            dataHorario: new DateTime(2025, 3, 16, 19, 0, 0));
        var sessao3 = CriarSessaoPadrao(
            id: 3,
            filme: filme,
            sala: sala2,
            dataHorario: new DateTime(2025, 3, 17, 19, 0, 0));

        sessaoServico.CriarSessao(sessao1);
        sessaoServico.CriarSessao(sessao2);
        sessaoServico.CriarSessao(sessao3);

        // Act
        var sessoes = sessaoServico.ListarSessoesPorSala(sala1.Id);

        // Assert
        sessoes.Should().HaveCount(2);
        sessoes.Should().AllSatisfy(s => s.Sala!.Id.Should().Be(sala1.Id));
    }

    // ==================== Testes de Atualização ====================

    [Fact]
    public void AtualizarSessao_ComDadosValidos_DeveAtualizarSessao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        sessaoServico.CriarSessao(sessao);

        var sessaoCriada = context.Sessoes.First();
        var novaData = new DateTime(2025, 4, 20, 14, 0, 0);
        var novoPreco = 75m;

        // Act
        sessaoServico.AtualizarSessao(
            sessaoCriada.Id,
            dataHorario: novaData,
            precoBase: novoPreco);

        // Assert
        var sessaoAtualizada = sessaoServico.ObterSessao(sessaoCriada.Id);
        sessaoAtualizada.DataHorario.Should().Be(novaData);
        sessaoAtualizada.PrecoBase.Should().Be(novoPreco);
    }

    [Fact]
    public void AtualizarSessao_ComIdInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        // Act & Assert
        var acao = () => sessaoServico.AtualizarSessao(999);
        
        acao.Should().Throw<RecursoNaoEncontradoExcecao>();
    }

    [Fact]
    public void AtualizarSessao_ComPrecoNegativo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        sessaoServico.CriarSessao(sessao);

        var sessaoCriada = context.Sessoes.First();

        // Act & Assert
        var acao = () => sessaoServico.AtualizarSessao(
            sessaoCriada.Id,
            precoBase: -50m);
        
        acao.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("*Preco base nao pode ser negativo*");
    }

    [Fact]
    public void AtualizarSessao_ApenasAlgunsParametros_DeveManterOutros()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(filme: filme, sala: sala, tipo: TipoSessao.Regular);
        sessaoServico.CriarSessao(sessao);

        var sessaoCriada = context.Sessoes.First();
        var precoOriginal = sessaoCriada.PrecoBase;

        // Act
        var novaData = new DateTime(2025, 4, 20, 14, 0, 0);
        sessaoServico.AtualizarSessao(sessaoCriada.Id, dataHorario: novaData);

        // Assert
        var sessaoAtualizada = sessaoServico.ObterSessao(sessaoCriada.Id);
        sessaoAtualizada.DataHorario.Should().Be(novaData);
        sessaoAtualizada.Tipo.Should().Be(TipoSessao.Regular);
    }

    // ==================== Testes de Deleção ====================

    [Fact]
    public void DeletarSessao_ComIdValido_DeveDeletarSessao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(filme: filme, sala: sala);
        sessaoServico.CriarSessao(sessao);

        var sessaoCriada = context.Sessoes.First();
        int idSessao = sessaoCriada.Id;

        // Act
        sessaoServico.DeletarSessao(idSessao);

        // Assert
        var acao = () => sessaoServico.ObterSessao(idSessao);
        acao.Should().Throw<RecursoNaoEncontradoExcecao>();
    }

    [Fact]
    public void DeletarSessao_ComIdInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        // Act & Assert
        var acao = () => sessaoServico.DeletarSessao(999);
        
        acao.Should().Throw<RecursoNaoEncontradoExcecao>();
    }

    // ==================== Testes de Cálculo de Preços ====================

    [Fact]
    public void CriarSessao_ComSalaVIP_DeveAplicarAdicionalVIP()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var garcom = CriarFuncionarioPadrao(
            id: 1,
            nome: "João Garçom",
            cargo: CargoFuncionario.Garcom,
            cinema: cinema);
        var salaVIP = CriarSalaPadrao(cinema: cinema, tipo: TipoSala.VIP);
        var filme = CriarFilmePadrao();

        cinema.Funcionarios.Add(garcom);
        context.Cinemas.Add(cinema);
        context.Salas.Add(salaVIP);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(
            filme: filme,
            sala: salaVIP,
            precoBase: 50m);

        // Act
        sessaoServico.CriarSessao(sessao);

        // Assert
        var sessaoCadastrada = context.Sessoes.First();
        // PreçoFinal deve ser maior que PreçoBase devido ao adicional VIP
        sessaoCadastrada.PrecoFinal.Should().BeGreaterThan(sessaoCadastrada.PrecoBase);
    }

    [Fact]
    public void CriarSessao_ComFilme3D_DeveAplicarAdicial3D()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme3D = CriarFilmePadrao(eh3D: true);

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme3D);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(
            filme: filme3D,
            sala: sala,
            precoBase: 50m);

        // Act
        sessaoServico.CriarSessao(sessao);

        // Assert
        var sessaoCadastrada = context.Sessoes.First();
        // PreçoFinal deve ser maior que PreçoBase devido ao adicional 3D
        sessaoCadastrada.PrecoFinal.Should().BeGreaterThan(sessaoCadastrada.PrecoBase);
    }

    [Fact]
    public void CriarSessao_TipoMatine_DeveAplicarDesconto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(
            filme: filme,
            sala: sala,
            precoBase: 50m,
            tipo: TipoSessao.Matine);

        // Act
        sessaoServico.CriarSessao(sessao);

        // Assert
        var sessaoCadastrada = context.Sessoes.First();
        // PreçoFinal deve ser menor que PreçoBase devido ao desconto matinê (25%)
        sessaoCadastrada.PrecoFinal.Should().BeLessThan(sessaoCadastrada.PrecoBase);
    }

    [Fact]
    public void CriarSessao_TipoPreEstreia_DeveAplicarAdicional()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var sessaoServico = new SessaoServico(context);

        var cinema = CriarCinemaPadrao();
        var gerente = CriarFuncionarioPadrao(
            id: 1,
            nome: "Maria Gerente",
            cargo: CargoFuncionario.Gerente,
            cinema: cinema);
        var sala = CriarSalaPadrao(cinema: cinema);
        var filme = CriarFilmePadrao();

        cinema.Funcionarios.Add(gerente);
        context.Cinemas.Add(cinema);
        context.Salas.Add(sala);
        context.Filmes.Add(filme);
        context.SaveChanges();

        var sessao = CriarSessaoPadrao(
            filme: filme,
            sala: sala,
            precoBase: 50m,
            tipo: TipoSessao.PreEstreia);

        // Act
        sessaoServico.CriarSessao(sessao);

        // Assert
        var sessaoCadastrada = context.Sessoes.First();
        // PreçoFinal deve ser maior que PreçoBase devido ao adicional pré-estreia
        sessaoCadastrada.PrecoFinal.Should().BeGreaterThan(sessaoCadastrada.PrecoBase);
    }
}
