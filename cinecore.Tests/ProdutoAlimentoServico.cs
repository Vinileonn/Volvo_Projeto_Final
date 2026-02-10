using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class ProdutoAlimentoServicoTests
{
    // Helper para criar um contexto de banco de dados em memória
    private CineFlowContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<CineFlowContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CineFlowContext(options);
    }

    private static ProdutoAlimento CriarProdutoPadrao(
        int id = 1,
        string? nome = null,
        string? descricao = null,
        CategoriaProduto? categoria = null,
        decimal preco = 10.00m,
        int estoqueAtual = 100,
        int estoqueMinimo = 10,
        bool ehTematico = false,
        string? temaFilme = null,
        bool ehCortesia = false,
        bool exclusivoPreEstreia = false)
    {
        return new ProdutoAlimento
        {
            Id = id,
            Nome = nome ?? "Produto Teste",
            Descricao = descricao,
            Categoria = categoria,
            Preco = preco,
            EstoqueAtual = estoqueAtual,
            EstoqueMinimo = estoqueMinimo,
            EhTematico = ehTematico,
            TemaFilme = temaFilme,
            EhCortesia = ehCortesia,
            ExclusivoPreEstreia = exclusivoPreEstreia
        };
    }

    #region Testes CriarProduto

    [Fact]
    public void CriarProduto_ComDadosValidos_DeveCriarComSucesso()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(nome: "Pipoca Grande");

        // Act
        servico.CriarProduto(produto);

        // Assert
        var produtoDb = context.ProdutosAlimento.FirstOrDefault(p => p.Id == produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.Nome.Should().Be("Pipoca Grande");
        produtoDb.Preco.Should().Be(10.00m);
        produtoDb.EstoqueAtual.Should().Be(100);
    }

    [Fact]
    public void CriarProduto_ComProdutoNulo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        Action act = () => servico.CriarProduto(null!);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Produto não pode ser nulo.");
    }

    [Fact]
    public void CriarProduto_ComNomeVazio_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(nome: "");

        // Act
        Action act = () => servico.CriarProduto(produto);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Dados do produto inválidos.");
    }

    [Fact]
    public void CriarProduto_ComPrecoNegativo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(preco: -5.00m);

        // Act
        Action act = () => servico.CriarProduto(produto);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Dados do produto inválidos.");
    }

    [Fact]
    public void CriarProduto_ComEstoqueAtualNegativo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueAtual: -10);

        // Act
        Action act = () => servico.CriarProduto(produto);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Dados do produto inválidos.");
    }

    [Fact]
    public void CriarProduto_ComEstoqueMinimoNegativo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueMinimo: -5);

        // Act
        Action act = () => servico.CriarProduto(produto);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Dados do produto inválidos.");
    }

    [Fact]
    public void CriarProduto_ComNomeDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto1 = CriarProdutoPadrao(id: 1, nome: "Coca-Cola");
        servico.CriarProduto(produto1);

        var produto2 = CriarProdutoPadrao(id: 2, nome: "Coca-Cola");

        // Act
        Action act = () => servico.CriarProduto(produto2);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Produto com o mesmo nome já existe.");
    }

    [Fact]
    public void CriarProduto_ComCategoriaCortesia_DeveDefinirEhCortesiaComoTrue()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(
            categoria: CategoriaProduto.Cortesia,
            preco: 0m,
            ehCortesia: false
        );

        // Act
        servico.CriarProduto(produto);

        // Assert
        var produtoDb = context.ProdutosAlimento.FirstOrDefault(p => p.Id == produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.EhCortesia.Should().BeTrue();
    }

    [Fact]
    public void CriarProduto_CortesiaComPrecoMaiorQueZero_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(ehCortesia: true, preco: 5.00m);

        // Act
        Action act = () => servico.CriarProduto(produto);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Cortesia deve ter preço zero.");
    }

    [Fact]
    public void CriarProduto_TematicoSemTemaFilme_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(ehTematico: true, temaFilme: null);

        // Act
        Action act = () => servico.CriarProduto(produto);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Produto temático precisa de tema do filme.");
    }

    [Fact]
    public void CriarProduto_CategoriaTematicoSemTemaFilme_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(categoria: CategoriaProduto.Tematico, temaFilme: null);

        // Act
        Action act = () => servico.CriarProduto(produto);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Produto temático precisa de tema do filme.");
    }

    [Fact]
    public void CriarProduto_TematicoComTemaFilme_DeveCriarComSucesso()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(
            nome: "Copo Avatar",
            ehTematico: true,
            temaFilme: "Avatar 2"
        );

        // Act
        servico.CriarProduto(produto);

        // Assert
        var produtoDb = context.ProdutosAlimento.FirstOrDefault(p => p.Id == produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.EhTematico.Should().BeTrue();
        produtoDb.TemaFilme.Should().Be("Avatar 2");
    }

    #endregion

    #region Testes ObterProduto

    [Fact]
    public void ObterProduto_ComIdValido_DeveRetornarProduto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(nome: "Pipoca");
        servico.CriarProduto(produto);

        // Act
        var resultado = servico.ObterProduto(produto.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(produto.Id);
        resultado.Nome.Should().Be("Pipoca");
    }

    [Fact]
    public void ObterProduto_ComIdInvalido_DeveRetornarNull()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        var resultado = servico.ObterProduto(999);

        // Assert
        resultado.Should().BeNull();
    }

    #endregion

    #region Testes ObterCortesiaPreEstreiaDisponivel

    [Fact]
    public void ObterCortesiaPreEstreiaDisponivel_ComCortesiaDisponivel_DeveRetornarProduto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(
            nome: "Brinde Pré-Estreia",
            preco: 0m,
            ehCortesia: true,
            exclusivoPreEstreia: true,
            estoqueAtual: 50
        );
        servico.CriarProduto(produto);

        // Act
        var resultado = servico.ObterCortesiaPreEstreiaDisponivel();

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Brinde Pré-Estreia");
        resultado.EhCortesia.Should().BeTrue();
        resultado.ExclusivoPreEstreia.Should().BeTrue();
    }

    [Fact]
    public void ObterCortesiaPreEstreiaDisponivel_SemEstoque_DeveRetornarNull()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(
            nome: "Brinde Pré-Estreia",
            preco: 0m,
            ehCortesia: true,
            exclusivoPreEstreia: true,
            estoqueAtual: 0
        );
        servico.CriarProduto(produto);

        // Act
        var resultado = servico.ObterCortesiaPreEstreiaDisponivel();

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void ObterCortesiaPreEstreiaDisponivel_SemCortesiaCadastrada_DeveRetornarNull()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        var resultado = servico.ObterCortesiaPreEstreiaDisponivel();

        // Assert
        resultado.Should().BeNull();
    }

    #endregion

    #region Testes ListarProdutos

    [Fact]
    public void ListarProdutos_ComProdutosCadastrados_DeveRetornarTodos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto1 = CriarProdutoPadrao(id: 1, nome: "Pipoca");
        var produto2 = CriarProdutoPadrao(id: 2, nome: "Refrigerante");
        var produto3 = CriarProdutoPadrao(id: 3, nome: "Chocolate");
        
        servico.CriarProduto(produto1);
        servico.CriarProduto(produto2);
        servico.CriarProduto(produto3);

        // Act
        var lista = servico.ListarProdutos();

        // Assert
        lista.Should().HaveCount(3);
        lista.Select(p => p.Nome).Should().Contain(new[] { "Pipoca", "Refrigerante", "Chocolate" });
    }

    [Fact]
    public void ListarProdutos_SemProdutosCadastrados_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        var lista = servico.ListarProdutos();

        // Assert
        lista.Should().BeEmpty();
    }

    #endregion

    #region Testes BuscarPorNome

    [Fact]
    public void BuscarPorNome_ComNomeExistente_DeveRetornarProdutos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto1 = CriarProdutoPadrao(id: 1, nome: "Pipoca Grande");
        var produto2 = CriarProdutoPadrao(id: 2, nome: "Pipoca Média");
        var produto3 = CriarProdutoPadrao(id: 3, nome: "Refrigerante");
        
        servico.CriarProduto(produto1);
        servico.CriarProduto(produto2);
        servico.CriarProduto(produto3);

        // Act
        var lista = servico.BuscarPorNome("Pipoca");

        // Assert
        lista.Should().HaveCount(2);
        lista.Select(p => p.Nome).Should().Contain(new[] { "Pipoca Grande", "Pipoca Média" });
    }

    [Fact]
    public void BuscarPorNome_ComNomeVazio_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(nome: "Pipoca");
        servico.CriarProduto(produto);

        // Act
        var lista = servico.BuscarPorNome("");

        // Assert
        lista.Should().BeEmpty();
    }

    [Fact]
    public void BuscarPorNome_ComNomeNaoEncontrado_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(nome: "Pipoca");
        servico.CriarProduto(produto);

        // Act
        var lista = servico.BuscarPorNome("Hamburger");

        // Assert
        lista.Should().BeEmpty();
    }

    #endregion

    #region Testes ListarProdutosEstoqueBaixo

    [Fact]
    public void ListarProdutosEstoqueBaixo_ComProdutosAbaixoDoMinimo_DeveRetornarProdutos()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto1 = CriarProdutoPadrao(id: 1, nome: "Produto A", estoqueAtual: 5, estoqueMinimo: 10);
        var produto2 = CriarProdutoPadrao(id: 2, nome: "Produto B", estoqueAtual: 10, estoqueMinimo: 10);
        var produto3 = CriarProdutoPadrao(id: 3, nome: "Produto C", estoqueAtual: 50, estoqueMinimo: 10);
        
        servico.CriarProduto(produto1);
        servico.CriarProduto(produto2);
        servico.CriarProduto(produto3);

        // Act
        var lista = servico.ListarProdutosEstoqueBaixo();

        // Assert
        lista.Should().HaveCount(2);
        lista.Select(p => p.Nome).Should().Contain(new[] { "Produto A", "Produto B" });
    }

    [Fact]
    public void ListarProdutosEstoqueBaixo_SemProdutosComEstoqueBaixo_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueAtual: 100, estoqueMinimo: 10);
        servico.CriarProduto(produto);

        // Act
        var lista = servico.ListarProdutosEstoqueBaixo();

        // Assert
        lista.Should().BeEmpty();
    }

    #endregion

    #region Testes ListarAlertasEstoque

    [Fact]
    public void ListarAlertasEstoque_DeveRetornarListaDeAlertas()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        var alertas = servico.ListarAlertasEstoque();

        // Assert
        alertas.Should().NotBeNull();
        alertas.Should().BeOfType<List<string>>();
    }

    #endregion

    #region Testes AtualizarProduto

    [Fact]
    public void AtualizarProduto_ComNomeValido_DeveAtualizarNome()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(nome: "Nome Original");
        servico.CriarProduto(produto);

        // Act
        servico.AtualizarProduto(produto.Id, nome: "Nome Atualizado");

        // Assert
        var produtoDb = context.ProdutosAlimento.Find(produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.Nome.Should().Be("Nome Atualizado");
    }

    [Fact]
    public void AtualizarProduto_ComDescricaoValida_DeveAtualizarDescricao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao();
        servico.CriarProduto(produto);

        // Act
        servico.AtualizarProduto(produto.Id, descricao: "Nova descrição");

        // Assert
        var produtoDb = context.ProdutosAlimento.Find(produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.Descricao.Should().Be("Nova descrição");
    }

    [Fact]
    public void AtualizarProduto_ComPrecoValido_DeveAtualizarPreco()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(preco: 10.00m);
        servico.CriarProduto(produto);

        // Act
        servico.AtualizarProduto(produto.Id, preco: 15.00m);

        // Assert
        var produtoDb = context.ProdutosAlimento.Find(produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.Preco.Should().Be(15.00m);
    }

    [Fact]
    public void AtualizarProduto_ComPrecoNegativo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao();
        servico.CriarProduto(produto);

        // Act
        Action act = () => servico.AtualizarProduto(produto.Id, preco: -5.00m);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Preço não pode ser negativo.");
    }

    [Fact]
    public void AtualizarProduto_ComEstoqueMinimoValido_DeveAtualizarEstoqueMinimo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueMinimo: 10);
        servico.CriarProduto(produto);

        // Act
        servico.AtualizarProduto(produto.Id, estoqueMinimo: 20);

        // Assert
        var produtoDb = context.ProdutosAlimento.Find(produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.EstoqueMinimo.Should().Be(20);
    }

    [Fact]
    public void AtualizarProduto_ComEstoqueMinimoNegativo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao();
        servico.CriarProduto(produto);

        // Act
        Action act = () => servico.AtualizarProduto(produto.Id, estoqueMinimo: -5);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Estoque mínimo não pode ser negativo.");
    }

    [Fact]
    public void AtualizarProduto_ComNomeDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto1 = CriarProdutoPadrao(id: 1, nome: "Produto A");
        var produto2 = CriarProdutoPadrao(id: 2, nome: "Produto B");
        servico.CriarProduto(produto1);
        servico.CriarProduto(produto2);

        // Act
        Action act = () => servico.AtualizarProduto(produto2.Id, nome: "Produto A");

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Produto com o mesmo nome já existe.");
    }

    [Fact]
    public void AtualizarProduto_CortesiaComPrecoMaiorQueZero_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(preco: 0m);
        servico.CriarProduto(produto);

        // Act
        Action act = () => servico.AtualizarProduto(produto.Id, ehCortesia: true, preco: 10.00m);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Cortesia deve ter preço zero.");
    }

    [Fact]
    public void AtualizarProduto_TematicoSemTemaFilme_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao();
        servico.CriarProduto(produto);

        // Act
        Action act = () => servico.AtualizarProduto(produto.Id, ehTematico: true, temaFilme: "");

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Produto temático precisa de tema do filme.");
    }

    [Fact]
    public void AtualizarProduto_ComIdInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        Action act = () => servico.AtualizarProduto(999, nome: "Novo Nome");

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Produto com ID 999 não encontrado.");
    }

    [Fact]
    public void AtualizarProduto_CategoriaCortesia_DeveDefinirEhCortesiaComoTrue()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(preco: 0m);
        servico.CriarProduto(produto);

        // Act
        servico.AtualizarProduto(produto.Id, categoria: CategoriaProduto.Cortesia);

        // Assert
        var produtoDb = context.ProdutosAlimento.Find(produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.EhCortesia.Should().BeTrue();
        produtoDb.Categoria.Should().Be(CategoriaProduto.Cortesia);
    }

    #endregion

    #region Testes DeletarProduto

    [Fact]
    public void DeletarProduto_ComIdValido_DeveDeletarComSucesso()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao();
        servico.CriarProduto(produto);

        // Act
        servico.DeletarProduto(produto.Id);

        // Assert
        var produtoDb = context.ProdutosAlimento.FirstOrDefault(p => p.Id == produto.Id);
        produtoDb.Should().BeNull();
    }

    [Fact]
    public void DeletarProduto_ComIdInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        Action act = () => servico.DeletarProduto(999);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Produto com ID 999 não encontrado.");
    }

    #endregion

    #region Testes AdicionarEstoque

    [Fact]
    public void AdicionarEstoque_ComQuantidadeValida_DeveAdicionarAoEstoque()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueAtual: 50);
        servico.CriarProduto(produto);

        // Act
        servico.AdicionarEstoque(produto.Id, 30);

        // Assert
        var produtoDb = context.ProdutosAlimento.Find(produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.EstoqueAtual.Should().Be(80);
    }

    [Fact]
    public void AdicionarEstoque_ComQuantidadeZero_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao();
        servico.CriarProduto(produto);

        // Act
        Action act = () => servico.AdicionarEstoque(produto.Id, 0);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void AdicionarEstoque_ComQuantidadeNegativa_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao();
        servico.CriarProduto(produto);

        // Act
        Action act = () => servico.AdicionarEstoque(produto.Id, -10);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void AdicionarEstoque_ComIdInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        Action act = () => servico.AdicionarEstoque(999, 10);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Produto com ID 999 não encontrado.");
    }

    #endregion

    #region Testes ReduzirEstoque

    [Fact]
    public void ReduzirEstoque_ComQuantidadeValida_DeveReduzirDoEstoque()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueAtual: 50);
        servico.CriarProduto(produto);

        // Act
        servico.ReduzirEstoque(produto.Id, 20);

        // Assert
        var produtoDb = context.ProdutosAlimento.Find(produto.Id);
        produtoDb.Should().NotBeNull();
        produtoDb!.EstoqueAtual.Should().Be(30);
    }

    [Fact]
    public void ReduzirEstoque_ComQuantidadeZero_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao();
        servico.CriarProduto(produto);

        // Act
        Action act = () => servico.ReduzirEstoque(produto.Id, 0);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void ReduzirEstoque_ComQuantidadeMaiorQueEstoque_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueAtual: 30);
        servico.CriarProduto(produto);

        // Act
        Action act = () => servico.ReduzirEstoque(produto.Id, 50);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade insuficiente em estoque.");
    }

    [Fact]
    public void ReduzirEstoque_AtingeEstoqueMinimo_DeveGerarAlerta()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(
            nome: "Pipoca",
            estoqueAtual: 30,
            estoqueMinimo: 10
        );
        servico.CriarProduto(produto);

        // Act
        servico.ReduzirEstoque(produto.Id, 20); // Fica com 10 (igual ao mínimo)

        // Assert
        var alertas = servico.ListarAlertasEstoque();
        alertas.Should().Contain(a => a.Contains("Pipoca") && a.Contains("10"));
    }

    [Fact]
    public void ReduzirEstoque_ComIdInvalido_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        Action act = () => servico.ReduzirEstoque(999, 10);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Produto com ID 999 não encontrado.");
    }

    #endregion

    #region Testes VerificarDisponibilidade

    [Fact]
    public void VerificarDisponibilidade_ComEstoqueSuficiente_DeveRetornarTrue()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueAtual: 50);
        servico.CriarProduto(produto);

        // Act
        var disponivel = servico.VerificarDisponibilidade(produto.Id, 30);

        // Assert
        disponivel.Should().BeTrue();
    }

    [Fact]
    public void VerificarDisponibilidade_ComEstoqueExato_DeveRetornarTrue()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueAtual: 50);
        servico.CriarProduto(produto);

        // Act
        var disponivel = servico.VerificarDisponibilidade(produto.Id, 50);

        // Assert
        disponivel.Should().BeTrue();
    }

    [Fact]
    public void VerificarDisponibilidade_ComEstoqueInsuficiente_DeveRetornarFalse()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);
        var produto = CriarProdutoPadrao(estoqueAtual: 30);
        servico.CriarProduto(produto);

        // Act
        var disponivel = servico.VerificarDisponibilidade(produto.Id, 50);

        // Assert
        disponivel.Should().BeFalse();
    }

    [Fact]
    public void VerificarDisponibilidade_ComIdInvalido_DeveRetornarFalse()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var servico = new ProdutoAlimentoServico(context);

        // Act
        var disponivel = servico.VerificarDisponibilidade(999, 10);

        // Assert
        disponivel.Should().BeFalse();
    }

    #endregion
}
