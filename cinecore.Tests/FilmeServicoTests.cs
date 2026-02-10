using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Enums;
using cinecore.Data;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class FilmeServicoTests
{
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    [Fact]
    public void CriarFilme_ComDadosValidos_DeveAdicionarNoContexto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        var filme = new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136,
            Classificacao = ClassificacaoIndicativa.Quatorze
        };

        // Act
        var resultado = filmeServico.CriarFilme(filme);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().BeGreaterThan(0);
        resultado.Titulo.Should().Be("Matrix");
        resultado.DataCriacao.Should().NotBe(DateTime.MinValue);
    }

    [Fact]
    public void CriarFilme_ComFilmeNulo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);

        // Act
        var acao = () => filmeServico.CriarFilme(null!);

        // Assert
        acao.Should().Throw<ArgumentNullException>()
            .WithMessage("*filme*");
    }

    [Fact]
    public void CriarFilme_SemTitulo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        var filme = new Filme
        {
            Titulo = "",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136
        };

        // Act
        var acao = () => filmeServico.CriarFilme(filme);

        // Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*título*");
    }

    [Fact]
    public void CriarFilme_ComDuracaoInvalida_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        var filme = new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 0  // Duração inválida
        };

        // Act
        var acao = () => filmeServico.CriarFilme(filme);

        // Assert
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*Duração*");
    }

    [Fact]
    public void CriarFilme_ComTituloDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        var filme1 = new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136
        };

        var filme2 = new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 140
        };

        filmeServico.CriarFilme(filme1);

        // Act
        var acao = () => filmeServico.CriarFilme(filme2);

        // Assert
        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("*já existe*");
    }

    [Fact]
    public void ObterFilme_ComIdValido_DeveRetornarFilme()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        var filme = new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136
        };
        
        var filmeCriado = filmeServico.CriarFilme(filme);

        // Act
        var resultado = filmeServico.ObterFilme(filmeCriado.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be("Matrix");
    }

    [Fact]
    public void ObterFilme_ComIdInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);

        // Act
        var acao = () => filmeServico.ObterFilme(9999);

        // Assert
        acao.Should().Throw<KeyNotFoundException>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public void ListarFilmes_SemFilmes_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);

        // Act
        var resultado = filmeServico.ListarFilmes();

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public void ListarFilmes_ComMultiplosFilmes_DeveRetornarTodos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        filmeServico.CriarFilme(new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136
        });

        filmeServico.CriarFilme(new Filme
        {
            Titulo = "Avatar",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(2009, 12, 18),
            Duracao = 162
        });

        // Act
        var resultado = filmeServico.ListarFilmes();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public void BuscarPorTitulo_ComTituloValido_DeveRetornarFilmes()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        filmeServico.CriarFilme(new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136
        });

        filmeServico.CriarFilme(new Filme
        {
            Titulo = "Avatar",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(2009, 12, 18),
            Duracao = 162
        });

        // Act
        var resultado = filmeServico.BuscarPorTitulo("Matrix");

        // Assert
        resultado.Should().HaveCount(1);
        resultado[0].Titulo.Should().Be("Matrix");
    }

    [Fact]
    public void BuscarPorTitulo_ComBuscaParcial_DeveRetornarFilmes()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        filmeServico.CriarFilme(new Filme
        {
            Titulo = "The Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136
        });

        // Act
        var resultado = filmeServico.BuscarPorTitulo("Matrix");

        // Assert
        resultado.Should().HaveCount(1);
    }

    [Fact]
    public void AtualizarFilme_ComDadosValidos_DeveAtualizar()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        var filme = filmeServico.CriarFilme(new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136
        });

        var filmeAtualizado = new Filme
        {
            Titulo = "The Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 138,
            Eh3D = true
        };

        // Act
        var resultado = filmeServico.AtualizarFilme(filme.Id, filmeAtualizado);

        // Assert
        resultado.Titulo.Should().Be("The Matrix");
        resultado.Duracao.Should().Be(138);
        resultado.Eh3D.Should().BeTrue();
    }

    [Fact]
    public void DeletarFilme_ComIdValido_DeveRemoverDoContexto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var filmeServico = new FilmeServico(context);
        
        var filme = filmeServico.CriarFilme(new Filme
        {
            Titulo = "Matrix",
            Genero = "Ficção Científica",
            AnoLancamento = new DateTime(1999, 3, 31),
            Duracao = 136
        });

        // Act
        filmeServico.DeletarFilme(filme.Id);

        // Assert
        var acao = () => filmeServico.ObterFilme(filme.Id);
        acao.Should().Throw<KeyNotFoundException>();
    }
}
