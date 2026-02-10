using Xunit;
using FluentAssertions;
using cinecore.Services;
using cinecore.Models;
using cinecore.Data;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Tests;

public class PedidoAlimentoServicoTests
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
        int id = 1,
        string nome = "Cliente Teste",
        string email = "cliente@teste.com",
        string senha = "senha123",
        string cpf = "12345678901",
        string telefone = "11999999999",
        string endereco = "Rua Teste, 123",
        DateTime? dataNascimento = null,
        int pontosFidelidade = 0)
    {
        return new Cliente
        {
            Id = id,
            Nome = nome,
            Email = email,
            Senha = senha,
            CPF = cpf,
            Telefone = telefone,
            Endereco = endereco,
            DataNascimento = dataNascimento ?? new DateTime(1990, 1, 15),
            PontosFidelidade = pontosFidelidade
        };
    }

    private static ProdutoAlimento CriarProdutoPadrao(
        int id = 1,
        string nome = "Produto Teste",
        decimal preco = 15.00m,
        int estoqueAtual = 100,
        int estoqueMinimo = 10,
        CategoriaProduto categoria = CategoriaProduto.Bebida)
    {
        return new ProdutoAlimento
        {
            Id = id,
            Nome = nome,
            Preco = preco,
            EstoqueAtual = estoqueAtual,
            EstoqueMinimo = estoqueMinimo,
            Categoria = categoria
        };
    }

    #region Testes CriarPedido

    [Fact]
    // Deve criar com sucesso
    public void CriarPedido_ComClienteValido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        var cliente = CriarClientePadrao();
        context.Usuarios.Add(cliente);
        context.SaveChanges();

        // Act
        var pedido = servico.CriarPedido(cliente);

        // Assert
        pedido.Should().NotBeNull();
        pedido.Id.Should().BeGreaterThan(0);
        pedido.Cliente.Should().Be(cliente);
        pedido.ValorTotal.Should().Be(0);
        pedido.Itens.Should().BeEmpty();
        pedido.DataPedido.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    // Deve lançar exceção
    public void CriarPedido_ComClienteNulo()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);

        // Act
        Action act = () => servico.CriarPedido(null!);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Cliente não pode ser nulo.");
    }

    #endregion

    #region Testes AdicionarItem

    [Fact]
    public void AdicionarItem_ComDadosValidos_DeveAdicionarComSucesso()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 20.00m, estoqueAtual: 50);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        servico.AdicionarItem(pedido.Id, produto.Id, 3);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado.Should().NotBeNull();
        pedidoAtualizado!.Itens.Should().HaveCount(1);
        pedidoAtualizado.Itens[0].Quantidade.Should().Be(3);
        pedidoAtualizado.Itens[0].Preco.Should().Be(20.00m);
        pedidoAtualizado.ValorTotal.Should().Be(60.00m);
        
        var produtoAtualizado = context.ProdutosAlimento.Find(produto.Id);
        produtoAtualizado!.EstoqueAtual.Should().Be(47);
    }

    [Fact]
    public void AdicionarItem_ComPrecoPersonalizado_DeveUsarPrecoInformado()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 20.00m, estoqueAtual: 50);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        servico.AdicionarItem(pedido.Id, produto.Id, 2, 15.00m);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado!.Itens[0].Preco.Should().Be(15.00m);
        pedidoAtualizado.ValorTotal.Should().Be(30.00m);
    }

    [Fact]
    public void AdicionarItem_ComQuantidadeZero_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao();
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        Action act = () => servico.AdicionarItem(pedido.Id, produto.Id, 0);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void AdicionarItem_ComQuantidadeNegativa_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao();
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        Action act = () => servico.AdicionarItem(pedido.Id, produto.Id, -5);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Quantidade deve ser maior que zero.");
    }

    [Fact]
    public void AdicionarItem_ComEstoqueInsuficiente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(nome: "Pipoca", estoqueAtual: 5);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        Action act = () => servico.AdicionarItem(pedido.Id, produto.Id, 10);

        // Assert
        act.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("Estoque insuficiente. 'Pipoca': 5 disponível.");
    }

    [Fact]
    public void AdicionarItem_ComPedidoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var produto = CriarProdutoPadrao();
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        // Act
        Action act = () => servico.AdicionarItem(999, produto.Id, 1);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Pedido ou produto não encontrado.");
    }

    [Fact]
    public void AdicionarItem_ComProdutoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        context.Usuarios.Add(cliente);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        Action act = () => servico.AdicionarItem(pedido.Id, 999, 1);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>();
    }

    #endregion

    #region Testes ObterPedido

    [Fact]
    public void ObterPedido_ComIdValido_DeveRetornarPedido()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        context.Usuarios.Add(cliente);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        var resultado = servico.ObterPedido(pedido.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(pedido.Id);
        resultado.Cliente.Should().NotBeNull();
        resultado.Cliente!.Id.Should().Be(cliente.Id);
    }

    [Fact]
    public void ObterPedido_ComIdInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);

        // Act
        Action act = () => servico.ObterPedido(999);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Pedido com ID 999 não encontrado.");
    }

    #endregion

    #region Testes ListarPedidos

    [Fact]
    public void ListarPedidos_ComPedidosCadastrados_DeveRetornarLista()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente1 = CriarClientePadrao(id: 1, email: "cliente1@teste.com");
        var cliente2 = CriarClientePadrao(id: 2, email: "cliente2@teste.com");
        
        context.Usuarios.AddRange(cliente1, cliente2);
        context.SaveChanges();

        servico.CriarPedido(cliente1);
        servico.CriarPedido(cliente2);

        // Act
        var pedidos = servico.ListarPedidos();

        // Assert
        pedidos.Should().NotBeNull();
        pedidos.Should().HaveCount(2);
        pedidos.All(p => p.Cliente != null).Should().BeTrue();
    }

    [Fact]
    public void ListarPedidos_SemPedidos_DeveRetornarListaVazia()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);

        // Act
        var pedidos = servico.ListarPedidos();

        // Assert
        pedidos.Should().NotBeNull();
        pedidos.Should().BeEmpty();
    }

    #endregion

    #region Testes RemoverItem

    [Fact]
    public void RemoverItem_ComItemValido_DeveRemoverEDevolverEstoque()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 10.00m, estoqueAtual: 50);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 5);
        
        var pedidoComItem = servico.ObterPedido(pedido.Id);
        var itemId = pedidoComItem!.Itens[0].Id;

        // Act
        servico.RemoverItem(pedido.Id, itemId);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado!.Itens.Should().BeEmpty();
        pedidoAtualizado.ValorTotal.Should().Be(0);
        
        var produtoAtualizado = context.ProdutosAlimento.Find(produto.Id);
        produtoAtualizado!.EstoqueAtual.Should().Be(50);
    }

    [Fact]
    public void RemoverItem_ComItemInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        context.Usuarios.Add(cliente);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        Action act = () => servico.RemoverItem(pedido.Id, 999);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Item com ID 999 não encontrado no pedido.");
    }

    [Fact]
    public void RemoverItem_ComPedidoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);

        // Act
        Action act = () => servico.RemoverItem(999, 1);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Pedido com ID 999 não encontrado.");
    }

    #endregion

    #region Testes CancelarPedido

    [Fact]
    public void CancelarPedido_NaoPago_DeveCancelarEDevolverEstoque()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(estoqueAtual: 50);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 10);

        // Act
        servico.CancelarPedido(pedido.Id);

        // Assert
        Action act = () => servico.ObterPedido(pedido.Id);
        act.Should().Throw<RecursoNaoEncontradoExcecao>();
        
        var produtoAtualizado = context.ProdutosAlimento.Find(produto.Id);
        produtoAtualizado!.EstoqueAtual.Should().Be(50);
    }

    [Fact]
    public void CancelarPedido_PagoDentroDoPrazo_DeveCancelar()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 10.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 2);
        servico.RegistrarPagamento(pedido.Id, FormaPagamento.Credito);

        // Act
        servico.CancelarPedido(pedido.Id);

        // Assert
        Action act = () => servico.ObterPedido(pedido.Id);
        act.Should().Throw<RecursoNaoEncontradoExcecao>();
    }

    [Fact]
    public void CancelarPedido_PagoForaDoPrazo_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 10.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 2);
        servico.RegistrarPagamento(pedido.Id, FormaPagamento.Credito);

        // Simular passagem de tempo (mais de 15 minutos)
        var pedidoDb = context.PedidosAlimento.Find(pedido.Id);
        pedidoDb!.DataPedido = DateTime.Now.AddMinutes(-20);
        context.SaveChanges();

        // Act
        Action act = () => servico.CancelarPedido(pedido.Id);

        // Assert
        act.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("Cancelamento permitido apenas ate 15 minutos apos o pedido pago.");
    }

    [Fact]
    public void CancelarPedido_ComPedidoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);

        // Act
        Action act = () => servico.CancelarPedido(999);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Pedido com ID 999 não encontrado.");
    }

    #endregion

    #region Testes CalcularTotal

    [Fact]
    public void CalcularTotal_ComPedidoValido_DeveRetornarValorCorreto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto1 = CriarProdutoPadrao(id: 1, preco: 10.00m);
        var produto2 = CriarProdutoPadrao(id: 2, preco: 15.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.AddRange(produto1, produto2);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto1.Id, 2); // 20.00
        servico.AdicionarItem(pedido.Id, produto2.Id, 3); // 45.00

        // Act
        var total = servico.CalcularTotal(pedido.Id);

        // Assert
        total.Should().Be(65.00m);
    }

    [Fact]
    public void CalcularTotal_ComPedidoVazio_DeveRetornarZero()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        context.Usuarios.Add(cliente);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);

        // Act
        var total = servico.CalcularTotal(pedido.Id);

        // Assert
        total.Should().Be(0m);
    }

    [Fact]
    public void CalcularTotal_ComPedidoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);

        // Act
        Action act = () => servico.CalcularTotal(999);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Pedido com ID 999 não encontrado.");
    }

    #endregion

    #region Testes RegistrarPagamento

    [Fact]
    public void RegistrarPagamento_ComCartao_DeveGerarPontosFidelidade()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao(pontosFidelidade: 0);
        var produto = CriarProdutoPadrao(preco: 25.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 1); // Total: 25.00

        // Act
        servico.RegistrarPagamento(pedido.Id, FormaPagamento.Credito);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado!.FormaPagamento.Should().Be(FormaPagamento.Credito);
        pedidoAtualizado.ValorPago.Should().Be(25.00m);
        pedidoAtualizado.PontosGerados.Should().Be(25);
        
        var clienteAtualizado = context.Usuarios.Find(cliente.Id) as Cliente;
        clienteAtualizado!.PontosFidelidade.Should().Be(25);
    }

    [Fact]
    public void RegistrarPagamento_ComDinheiroValorExato_DeveTerTrocoZero()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 20.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 1);

        // Act
        servico.RegistrarPagamento(pedido.Id, FormaPagamento.Dinheiro, 20.00m);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado!.FormaPagamento.Should().Be(FormaPagamento.Dinheiro);
        pedidoAtualizado.ValorPago.Should().Be(20.00m);
        pedidoAtualizado.ValorTroco.Should().Be(0m);
    }

    [Fact]
    public void RegistrarPagamento_ComDinheiroEMaior_DeveCalcularTroco()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 18.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 1);

        // Act
        servico.RegistrarPagamento(pedido.Id, FormaPagamento.Dinheiro, 50.00m);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado!.FormaPagamento.Should().Be(FormaPagamento.Dinheiro);
        pedidoAtualizado.ValorPago.Should().Be(50.00m);
        pedidoAtualizado.ValorTroco.Should().Be(32.00m);
        pedidoAtualizado.TrocoDetalhado.Should().NotBeEmpty();
    }

    [Fact]
    public void RegistrarPagamento_ComDinheiroInsuficiente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 30.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 1);

        // Act
        Action act = () => servico.RegistrarPagamento(pedido.Id, FormaPagamento.Dinheiro, 20.00m);

        // Assert
        act.Should().Throw<DadosInvalidosExcecao>()
            .WithMessage("Valor pago menor que o total.");
    }

    [Fact]
    public void RegistrarPagamento_ComDescontoAniversario_DeveAplicarDesconto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        // Cliente com aniversário no mês atual
        var mesAtual = DateTime.Now.Month;
        var dataNascimento = new DateTime(1990, mesAtual, 15);
        var cliente = CriarClientePadrao(dataNascimento: dataNascimento);
        var produto = CriarProdutoPadrao(preco: 100.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 1); // Total: 100.00

        // Act
        servico.RegistrarPagamento(pedido.Id, FormaPagamento.Credito);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado!.ValorDesconto.Should().Be(10.00m); // 10% de desconto
        pedidoAtualizado.MotivoDesconto.Should().Be("Desconto de aniversario");
        pedidoAtualizado.ValorTotal.Should().Be(90.00m);
        pedidoAtualizado.ValorPago.Should().Be(90.00m);
    }

    [Fact]
    public void RegistrarPagamento_ComPontosFidelidade_DeveAplicarDesconto()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao(pontosFidelidade: 100);
        var produto = CriarProdutoPadrao(preco: 50.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 1); // Total: 50.00

        // Act - Usar 50 pontos (50 * 0.10 = 5.00 de desconto)
        servico.RegistrarPagamento(pedido.Id, FormaPagamento.Credito, 0m, 50);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado!.PontosUsados.Should().Be(50);
        pedidoAtualizado.ValorTotal.Should().Be(45.00m); // 50 - 5
        
        var clienteAtualizado = context.Usuarios.Find(cliente.Id) as Cliente;
        clienteAtualizado!.PontosFidelidade.Should().Be(95); // 100 - 50 (usados) + 45 (ganhos do pedido)
    }

    [Fact]
    public void RegistrarPagamento_ComPontosInsuficientes_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao(pontosFidelidade: 20);
        var produto = CriarProdutoPadrao(preco: 30.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 1);

        // Act - Tentar usar 50 pontos quando tem apenas 20
        Action act = () => servico.RegistrarPagamento(pedido.Id, FormaPagamento.Credito, 0m, 50);

        // Assert
        act.Should().Throw<OperacaoNaoPermitidaExcecao>()
            .WithMessage("Pontos insuficientes.");
    }

    [Fact]
    public void RegistrarPagamento_ComPedidoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);

        // Act
        Action act = () => servico.RegistrarPagamento(999, FormaPagamento.Credito);

        // Assert
        act.Should().Throw<RecursoNaoEncontradoExcecao>()
            .WithMessage("Pedido com ID 999 não encontrado.");
    }

    [Fact]
    public void RegistrarPagamento_ComPix_DeveFuncionarCorretamente()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var produtoServico = new ProdutoAlimentoServico(context);
        var servico = new PedidoAlimentoServico(context, produtoServico);
        
        var cliente = CriarClientePadrao();
        var produto = CriarProdutoPadrao(preco: 35.00m);
        
        context.Usuarios.Add(cliente);
        context.ProdutosAlimento.Add(produto);
        context.SaveChanges();

        var pedido = servico.CriarPedido(cliente);
        servico.AdicionarItem(pedido.Id, produto.Id, 1);

        // Act
        servico.RegistrarPagamento(pedido.Id, FormaPagamento.Pix);

        // Assert
        var pedidoAtualizado = servico.ObterPedido(pedido.Id);
        pedidoAtualizado!.FormaPagamento.Should().Be(FormaPagamento.Pix);
        pedidoAtualizado.ValorPago.Should().Be(35.00m);
        pedidoAtualizado.ValorTroco.Should().Be(0m);
        pedidoAtualizado.PontosGerados.Should().Be(35);
    }

    #endregion
}
