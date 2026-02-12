using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class LimpezaServicoTests
{
    // Helper para criar um contexto de banco de dados em memória
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    private static Sala CriarSalaPadrao(int id = 1, string nome = "Sala 1", int capacidade = 100)
    {
        return new Sala
        {
            Id = id,
            Nome = nome,
            Capacidade = capacidade,
            Tipo = TipoSala.Normal
        };
    }

    private static Funcionario CriarFuncionarioPadrao(
        int id = 1,
        string nome = "João Silva",
        CargoFuncionario cargo = CargoFuncionario.Limpeza)
    {
        return new Funcionario
        {
            Id = id,
            Nome = nome,
            Cargo = cargo
        };
    }

    #region Testes CriarEscala

    [Fact]
    // Deve criar com sucesso
    public void CriarEscalaComDadosValidos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao();
        var inicio = DateTime.Now;
        var fim = inicio.AddHours(2);

        // Act
        var escala = servico.CriarEscala(sala, funcionario, inicio, fim);

        // Assert
        escala.Should().NotBeNull();
        escala.Sala.Should().Be(sala);
        escala.Funcionario.Should().Be(funcionario);
        escala.Inicio.Should().Be(inicio);
        escala.Fim.Should().Be(fim);
        
        var escalaDb = context.EscalasLimpeza.FirstOrDefault(e => e.Id == escala.Id);
        escalaDb.Should().NotBeNull();
    }

    [Fact]
    // Deve lançar exceção
    public void CriarEscalaComSalaNula()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var funcionario = CriarFuncionarioPadrao();
        var inicio = DateTime.Now;
        var fim = inicio.AddHours(2);

        // Act
        Action act = () => servico.CriarEscala(null!, funcionario, inicio, fim);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Sala e funcionario sao obrigatorios.");
    }

    [Fact]
    public void CriarEscalaComFuncionarioNulo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var inicio = DateTime.Now;
        var fim = inicio.AddHours(2);

        // Act
        Action act = () => servico.CriarEscala(sala, null!, inicio, fim);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Sala e funcionario sao obrigatorios.");
    }

    [Fact]
    public void CriarEscalaComFuncionarioNaoDaLimpeza()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao(cargo: CargoFuncionario.AtendenteIngressos);
        var inicio = DateTime.Now;
        var fim = inicio.AddHours(2);

        // Act
        Action act = () => servico.CriarEscala(sala, funcionario, inicio, fim);

        // Assert
        act.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("Funcionario nao e da limpeza.");
    }

    [Fact]
    public void CriarEscalaComInicioDefault()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao();
        var fim = DateTime.Now.AddHours(2);

        // Act
        Action act = () => servico.CriarEscala(sala, funcionario, default, fim);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Periodo de limpeza invalido.");
    }

    [Fact]
    public void CriarEscalaComFimDefault()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao();
        var inicio = DateTime.Now;

        // Act
        Action act = () => servico.CriarEscala(sala, funcionario, inicio, default);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Periodo de limpeza invalido.");
    }

    [Fact]
    public void CriarEscalaComInicioMaiorOuIgualFim()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao();
        var inicio = DateTime.Now;
        var fim = inicio; // Igual ao início

        // Act
        Action act = () => servico.CriarEscala(sala, funcionario, inicio, fim);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Periodo de limpeza invalido.");
    }

    [Fact]
    public void CriarEscalaComConflitoDeHorarioNaMesmaSala()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario1 = CriarFuncionarioPadrao(id: 1, nome: "João");
        var funcionario2 = CriarFuncionarioPadrao(id: 2, nome: "Maria");
        var inicio1 = DateTime.Now;
        var fim1 = inicio1.AddHours(2);

        // Criar primeira escala
        servico.CriarEscala(sala, funcionario1, inicio1, fim1);

        // Tentar criar segunda escala com conflito de horário na mesma sala
        var inicio2 = inicio1.AddHours(1); // Sobrepõe
        var fim2 = fim1.AddHours(1);

        // Act
        Action act = () => servico.CriarEscala(sala, funcionario2, inicio2, fim2);

        // Assert
        act.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("Conflito de horario na escala de limpeza.");
    }

    [Fact]
    public void CriarEscalaComConflitoDeHorarioParaMesmoFuncionario()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala1 = CriarSalaPadrao(id: 1, nome: "Sala 1");
        var sala2 = CriarSalaPadrao(id: 2, nome: "Sala 2");
        var funcionario = CriarFuncionarioPadrao();
        var inicio1 = DateTime.Now;
        var fim1 = inicio1.AddHours(2);

        // Criar primeira escala
        servico.CriarEscala(sala1, funcionario, inicio1, fim1);

        // Tentar criar segunda escala com conflito de horário para o mesmo funcionário
        var inicio2 = inicio1.AddHours(1); // Sobrepõe
        var fim2 = fim1.AddHours(1);

        // Act
        Action act = () => servico.CriarEscala(sala2, funcionario, inicio2, fim2);

        // Assert
        act.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("Conflito de horario na escala de limpeza.");
    }

    [Fact]
    public void CriarEscalaSemConflitoDeHorario()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario1 = CriarFuncionarioPadrao(id: 1, nome: "João");
        var funcionario2 = CriarFuncionarioPadrao(id: 2, nome: "Maria");
        var inicio1 = DateTime.Now;
        var fim1 = inicio1.AddHours(2);

        // Criar primeira escala
        servico.CriarEscala(sala, funcionario1, inicio1, fim1);

        // Criar segunda escala sem conflito (hora após a primeira)
        var inicio2 = fim1.AddMinutes(1);
        var fim2 = inicio2.AddHours(2);

        // Act
        var escala2 = servico.CriarEscala(sala, funcionario2, inicio2, fim2);

        // Assert
        escala2.Should().NotBeNull();
        context.EscalasLimpeza.Should().HaveCount(2);
    }

    #endregion

    #region Testes ListarEscalas

    [Fact]
    public void ListarEscalasQuandoNaoHaEscalas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);

        // Act
        var escalas = servico.ListarEscalas();

        // Assert
        escalas.Should().BeEmpty();
    }

    [Fact]
    public void ListarEscalasQuandoHaEscalas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala1 = CriarSalaPadrao(id: 1, nome: "Sala 1");
        var sala2 = CriarSalaPadrao(id: 2, nome: "Sala 2");
        var funcionario = CriarFuncionarioPadrao();
        
        var inicio1 = DateTime.Now;
        var fim1 = inicio1.AddHours(2);
        var inicio2 = fim1.AddHours(1);
        var fim2 = inicio2.AddHours(2);

        servico.CriarEscala(sala1, funcionario, inicio1, fim1);
        servico.CriarEscala(sala2, funcionario, inicio2, fim2);

        // Act
        var escalas = servico.ListarEscalas();

        // Assert
        escalas.Should().HaveCount(2);
        escalas.Should().Contain(e => e.Sala!.Id == sala1.Id);
        escalas.Should().Contain(e => e.Sala!.Id == sala2.Id);
    }

    #endregion

    #region Testes ListarPorSala

    [Fact]
    public void ListarPorSalaQuandoNaoHaEscalasParaSala()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao();
        
        servico.CriarEscala(sala, funcionario, DateTime.Now, DateTime.Now.AddHours(2));

        // Act
        var escalas = servico.ListarPorSala(99); // ID que não existe

        // Assert
        escalas.Should().BeEmpty();
    }

    [Fact]
    public void ListarPorSalaQuandoHaEscalasParaSala()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala1 = CriarSalaPadrao(id: 1, nome: "Sala 1");
        var sala2 = CriarSalaPadrao(id: 2, nome: "Sala 2");
        var funcionario = CriarFuncionarioPadrao();
        
        var inicio1 = DateTime.Now;
        var fim1 = inicio1.AddHours(2);
        var inicio2 = fim1.AddHours(1);
        var fim2 = inicio2.AddHours(2);
        var inicio3 = fim2.AddHours(1);
        var fim3 = inicio3.AddHours(2);

        servico.CriarEscala(sala1, funcionario, inicio1, fim1);
        servico.CriarEscala(sala1, funcionario, inicio2, fim2);
        servico.CriarEscala(sala2, funcionario, inicio3, fim3);

        // Act
        var escalas = servico.ListarPorSala(sala1.Id);

        // Assert
        escalas.Should().HaveCount(2);
        escalas.Should().OnlyContain(e => e.Sala!.Id == sala1.Id);
    }

    #endregion

    #region Testes ListarPorFuncionario

    [Fact]
    public void ListarPorFuncionarioQuandoNaoHaEscalasParaFuncionario()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao();
        
        servico.CriarEscala(sala, funcionario, DateTime.Now, DateTime.Now.AddHours(2));

        // Act
        var escalas = servico.ListarPorFuncionario(99); // ID que não existe

        // Assert
        escalas.Should().BeEmpty();
    }

    [Fact]
    public void ListarPorFuncionarioQuandoHaEscalasParaFuncionario()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario1 = CriarFuncionarioPadrao(id: 1, nome: "João");
        var funcionario2 = CriarFuncionarioPadrao(id: 2, nome: "Maria");
        
        var inicio1 = DateTime.Now;
        var fim1 = inicio1.AddHours(2);
        var inicio2 = fim1.AddHours(1);
        var fim2 = inicio2.AddHours(2);
        var inicio3 = fim2.AddHours(1);
        var fim3 = inicio3.AddHours(2);

        servico.CriarEscala(sala, funcionario1, inicio1, fim1);
        servico.CriarEscala(sala, funcionario1, inicio2, fim2);
        servico.CriarEscala(sala, funcionario2, inicio3, fim3);

        // Act
        var escalas = servico.ListarPorFuncionario(funcionario1.Id);

        // Assert
        escalas.Should().HaveCount(2);
        escalas.Should().OnlyContain(e => e.Funcionario!.Id == funcionario1.Id);
    }

    #endregion

    #region Testes DeletarEscala

    [Fact]
    public void DeletarEscalaComIdValido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao();
        
        var escala = servico.CriarEscala(sala, funcionario, DateTime.Now, DateTime.Now.AddHours(2));

        // Act
        var escalaDeletada = servico.DeletarEscala(escala.Id);

        // Assert
        escalaDeletada.Should().NotBeNull();
        escalaDeletada.Id.Should().Be(escala.Id);
        
        var escalaDb = context.EscalasLimpeza.FirstOrDefault(e => e.Id == escala.Id);
        escalaDb.Should().BeNull();
    }

    [Fact]
    public void DeletarEscalaComIdInvalido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var idInvalido = 99;

        // Act
        Action act = () => servico.DeletarEscala(idInvalido);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage($"Escala com ID {idInvalido} nao encontrada.");
    }

    [Fact]
    public void DeletarEscalaAposDeletarNaoMaisEstarNaLista()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new LimpezaServico(context);
        var sala = CriarSalaPadrao();
        var funcionario = CriarFuncionarioPadrao();
        
        var inicio1 = DateTime.Now;
        var fim1 = inicio1.AddHours(2);
        var inicio2 = fim1.AddHours(1);
        var fim2 = inicio2.AddHours(2);

        var escala1 = servico.CriarEscala(sala, funcionario, inicio1, fim1);
        var escala2 = servico.CriarEscala(sala, funcionario, inicio2, fim2);

        // Act
        servico.DeletarEscala(escala1.Id);
        var escalas = servico.ListarEscalas();

        // Assert
        escalas.Should().HaveCount(1);
        escalas.Should().Contain(e => e.Id == escala2.Id);
        escalas.Should().NotContain(e => e.Id == escala1.Id);
    }

    #endregion
}
