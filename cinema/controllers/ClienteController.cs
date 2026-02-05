using cinema.models;
using cinema.models.UsuarioModel;
using cinema.models.IngressoModel;
using cinema.services;
using cinema.exceptions;

namespace cinema.controllers
{
    public class ClienteController
    {
        private readonly FilmeService filmeService;
        private readonly SessaoService sessaoService;
        private readonly IngressoService ingressoService;
        private readonly PedidoAlimentoService pedidoAlimentoService;
        private readonly ProdutoAlimentoService produtoAlimentoService;

        public ClienteController(
            FilmeService filmeService,
            SessaoService sessaoService,
            IngressoService ingressoService,
            PedidoAlimentoService pedidoAlimentoService,
            ProdutoAlimentoService produtoAlimentoService)
        {
            this.filmeService = filmeService;
            this.sessaoService = sessaoService;
            this.ingressoService = ingressoService;
            this.pedidoAlimentoService = pedidoAlimentoService;
            this.produtoAlimentoService = produtoAlimentoService;
        }

        // LEITURA - listar cartaz (filmes disponíveis)
        public (List<Filme> filmes, string mensagem) ListarCartaz()
        {
            try
            {
                var filmes = filmeService.ListarFilmes();
                if (filmes.Count == 0)
                {
                    return (filmes, "Nenhum filme disponível no cartaz.");
                }
                return (filmes, $"Cartaz carregado com sucesso. {filmes.Count} filme(s) disponível(is).");
            }
            catch (Exception)
            {
                return (new List<Filme>(), "Erro inesperado ao carregar o cartaz.");
            }
        }

        // LEITURA - listar sessões de um filme
        public (List<Sessao> sessoes, string mensagem) ListarSessoesPorFilme(int filmeId)
        {
            try
            {
                var filme = filmeService.ObterFilme(filmeId);
                var sessoes = filme.Sessoes;
                if (sessoes.Count == 0)
                {
                    return (sessoes, $"Nenhuma sessão disponível para '{filme.Titulo}'.");
                }
                return (sessoes, $"Sessões carregadas com sucesso. {sessoes.Count} sessão(ões) disponível(is).");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (new List<Sessao>(), $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (new List<Sessao>(), "Erro inesperado ao carregar as sessões.");
            }
        }

        // LEITURA - listar todas as sessões
        public (List<Sessao> sessoes, string mensagem) ListarTodasSessoes()
        {
            try
            {
                var sessoes = sessaoService.ListarSessoes();
                if (sessoes.Count == 0)
                {
                    return (sessoes, "Nenhuma sessão disponível.");
                }
                return (sessoes, $"Sessões carregadas com sucesso. {sessoes.Count} sessão(ões) disponível(is).");
            }
            catch (Exception)
            {
                return (new List<Sessao>(), "Erro inesperado ao carregar as sessões.");
            }
        }

        // COMPRA - venda de ingresso inteiro
        public (Ingresso? ingresso, string mensagem) ComprarIngressoInteiro(Sessao sessao, Cliente cliente, char fila, int numero)
        {
            try
            {
                if (sessao == null || cliente == null)
                {
                    return (null, "Dados inválidos: sessão ou cliente nulo.");
                }

                var ingresso = ingressoService.VenderInteira(sessao, cliente, fila, numero);
                return (ingresso, $"Ingresso inteiro comprado com sucesso! Assento: {fila}{numero}");
            }
            catch (DadosInvalidosException ex)
            {
                return (null, $"Dados inválidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
            {
                return (null, $"Operação não permitida: {ex.Message}");
            }
            catch (ErroOperacaoCriticaException ex)
            {
                return (null, $"Erro crítico na operação: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao comprar ingresso inteiro.");
            }
        }

        // COMPRA - venda de meia entrada
        public (Ingresso? ingresso, string mensagem) ComprarIngressoMeia(Sessao sessao, Cliente cliente, char fila, int numero, string motivo)
        {
            try
            {
                if (sessao == null || cliente == null)
                {
                    return (null, "Dados inválidos: sessão ou cliente nulo.");
                }

                var ingresso = ingressoService.VenderMeia(sessao, cliente, fila, numero, motivo);
                return (ingresso, $"Meia entrada comprada com sucesso! Assento: {fila}{numero}. Motivo: {motivo}");
            }
            catch (DadosInvalidosException ex)
            {
                return (null, $"Dados inválidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
            {
                return (null, $"Operação não permitida: {ex.Message}");
            }
            catch (ErroOperacaoCriticaException ex)
            {
                return (null, $"Erro crítico na operação: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao comprar meia entrada.");
            }
        }

        // LEITURA - listar produtos de alimento
        public (List<ProdutoAlimento> produtos, string mensagem) ListarProdutosAlimento()
        {
            try
            {
                var produtos = produtoAlimentoService.ListarProdutos();
                if (produtos.Count == 0)
                {
                    return (produtos, "Nenhum produto de alimento disponível.");
                }
                return (produtos, $"Produtos carregados com sucesso. {produtos.Count} produto(s) disponível(is).");
            }
            catch (Exception)
            {
                return (new List<ProdutoAlimento>(), "Erro inesperado ao carregar produtos.");
            }
        }

        // PEDIDO - criar novo pedido de alimento
        public (PedidoAlimento? pedido, string mensagem) CriarPedidoAlimento(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                {
                    return (null, "Cliente não pode ser nulo.");
                }

                var pedido = pedidoAlimentoService.CriarPedido(cliente);
                return (pedido, "Pedido de alimento criado com sucesso.");
            }
            catch (DadosInvalidosException ex)
            {
                return (null, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao criar pedido.");
            }
        }

        // PEDIDO - adicionar item ao pedido de alimento
        public (bool sucesso, string mensagem) AdicionarItemAoPedido(int pedidoId, int produtoId, int quantidade)
        {
            try
            {
                pedidoAlimentoService.AdicionarItem(pedidoId, produtoId, quantidade);
                return (true, "Item adicionado ao pedido com sucesso.");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (ErroOperacaoCriticaException ex)
            {
                return (false, $"Erro crítico na operação: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao adicionar item ao pedido.");
            }
        }

        // PEDIDO - remover item do pedido
        public (bool sucesso, string mensagem) RemoverItemDoPedido(int pedidoId, int itemId)
        {
            try
            {
                pedidoAlimentoService.RemoverItem(pedidoId, itemId);
                return (true, "Item removido do pedido com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao remover item do pedido.");
            }
        }

        // PEDIDO - obter pedido por id
        public (PedidoAlimento? pedido, string mensagem) ObterPedido(int pedidoId)
        {
            try
            {
                var pedido = pedidoAlimentoService.ObterPedido(pedidoId);
                return (pedido, "Pedido obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter pedido.");
            }
        }

        // PEDIDO - listar todos os pedidos
        public (List<PedidoAlimento> pedidos, string mensagem) ListarPedidos()
        {
            try
            {
                var pedidos = pedidoAlimentoService.ListarPedidos();
                if (pedidos.Count == 0)
                {
                    return (pedidos, "Nenhum pedido disponível.");
                }
                return (pedidos, $"Pedidos carregados com sucesso. {pedidos.Count} pedido(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<PedidoAlimento>(), "Erro inesperado ao listar pedidos.");
            }
        }

        // PEDIDO - calcular total do pedido
        public (float total, string mensagem) CalcularTotalPedido(int pedidoId)
        {
            try
            {
                var total = pedidoAlimentoService.CalcularTotal(pedidoId);
                return (total, $"Total calculado com sucesso: R$ {total:F2}");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (0, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (0, "Erro inesperado ao calcular total.");
            }
        }

        // LEITURA - listar ingressos do cliente
        public (List<Ingresso> ingressos, string mensagem) ListarIngressosDoCliente(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                {
                    return (new List<Ingresso>(), "Cliente não pode ser nulo.");
                }

                var ingressos = cliente.Ingressos ?? new List<Ingresso>();
                if (ingressos.Count == 0)
                {
                    return (ingressos, "Nenhum ingresso comprado.");
                }
                return (ingressos, $"Ingressos carregados com sucesso. {ingressos.Count} ingresso(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<Ingresso>(), "Erro inesperado ao listar ingressos.");
            }
        }
    }
}
