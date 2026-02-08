using cineflow.modelos;
using cineflow.modelos.UsuarioModelo;
using cineflow.modelos.IngressoModelo;
using cineflow.servicos;
using cineflow.enumeracoes;
using cineflow.excecoes;

namespace cineflow.controladores
{
    public class ClienteControlador
    {
        private readonly FilmeServico FilmeServico;
        private readonly SessaoServico SessaoServico;
        private readonly IngressoServico IngressoServico;
        private readonly PedidoAlimentoServico PedidoAlimentoServico;
        private readonly ProdutoAlimentoServico ProdutoAlimentoServico;

        public ClienteControlador(
            FilmeServico FilmeServico,
            SessaoServico SessaoServico,
            IngressoServico IngressoServico,
            PedidoAlimentoServico PedidoAlimentoServico,
            ProdutoAlimentoServico ProdutoAlimentoServico)
        {
            this.FilmeServico = FilmeServico;
            this.SessaoServico = SessaoServico;
            this.IngressoServico = IngressoServico;
            this.PedidoAlimentoServico = PedidoAlimentoServico;
            this.ProdutoAlimentoServico = ProdutoAlimentoServico;
        }

        // LEITURA - listar cartaz (filmes disponíveis)
        public (List<Filme> filmes, string mensagem) ListarCartaz()
        {
            try
            {
                var filmes = FilmeServico.ListarFilmes();
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
                var filme = FilmeServico.ObterFilme(filmeId);
                var sessoes = filme.Sessoes;
                if (sessoes.Count == 0)
                {
                    return (sessoes, $"Nenhuma sessão disponível para '{filme.Titulo}'.");
                }
                return (sessoes, $"Sessões carregadas com sucesso. {sessoes.Count} sessão(ões) disponível(is).");
            }
            catch (RecursoNaoEncontradoExcecao ex)
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
                var sessoes = SessaoServico.ListarSessoes();
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
        public (Ingresso? ingresso, string mensagem) ComprarIngressoInteiro(Sessao sessao, Cliente cliente, char fila, int numero,
            FormaPagamento formaPagamento, decimal valorPago = 0m, string? cupomParceiro = null, bool reservaAntecipada = false,
            int pontosUsados = 0)
        {
            try
            {
                if (sessao == null || cliente == null)
                {
                    return (null, "Dados invalidos: sessao ou cliente nulo.");
                }

                var ingresso = IngressoServico.VenderInteira(sessao, cliente, fila, numero, formaPagamento, valorPago,
                    cupomParceiro, reservaAntecipada, pontosUsados);
                var mensagem = $"Ingresso inteiro comprado com sucesso! Assento: {fila}{numero}";
                var cortesiaMensagem = ConcederCortesiaPreEstreia(sessao, cliente);
                if (!string.IsNullOrWhiteSpace(cortesiaMensagem))
                {
                    mensagem += $" | {cortesiaMensagem}";
                }
                return (ingresso, mensagem);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (null, $"Dados invalidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (null, $"Operacao nao permitida: {ex.Message}");
            }
            catch (ErroOperacaoCriticaExcecao ex)
            {
                return (null, $"Erro critico na operacao: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao comprar ingresso inteiro.");
            }
        }

        // COMPRA - venda de meia entrada
        public (Ingresso? ingresso, string mensagem) ComprarIngressoMeia(Sessao sessao, Cliente cliente, char fila, int numero, string motivo,
            FormaPagamento formaPagamento, decimal valorPago = 0m, string? cupomParceiro = null, bool reservaAntecipada = false,
            int pontosUsados = 0)
        {
            try
            {
                if (sessao == null || cliente == null)
                {
                    return (null, "Dados invalidos: sessao ou cliente nulo.");
                }

                var ingresso = IngressoServico.VenderMeia(sessao, cliente, fila, numero, motivo, formaPagamento, valorPago,
                    cupomParceiro, reservaAntecipada, pontosUsados);
                var mensagem = $"Meia entrada comprada com sucesso! Assento: {fila}{numero}. Motivo: {motivo}";
                var cortesiaMensagem = ConcederCortesiaPreEstreia(sessao, cliente);
                if (!string.IsNullOrWhiteSpace(cortesiaMensagem))
                {
                    mensagem += $" | {cortesiaMensagem}";
                }
                return (ingresso, mensagem);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (null, $"Dados invalidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (null, $"Operacao nao permitida: {ex.Message}");
            }
            catch (ErroOperacaoCriticaExcecao ex)
            {
                return (null, $"Erro critico na operacao: {ex.Message}");
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
                var produtos = ProdutoAlimentoServico.ListarProdutos();
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

                var pedido = PedidoAlimentoServico.CriarPedido(cliente);
                return (pedido, "Pedido de alimento criado com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (null, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao criar pedido.");
            }
        }

        // PEDIDO - adicionar item ao pedido de alimento
        public (bool sucesso, string mensagem) AdicionarItemAoPedido(int pedidoId, int produtoId, int quantidade, float? precoUnitario = null)
        {
            try
            {
                PedidoAlimentoServico.AdicionarItem(pedidoId, produtoId, quantidade, precoUnitario);
                return (true, "Item adicionado ao pedido com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (ErroOperacaoCriticaExcecao ex)
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
                PedidoAlimentoServico.RemoverItem(pedidoId, itemId);
                return (true, "Item removido do pedido com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
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
                var pedido = PedidoAlimentoServico.ObterPedido(pedidoId);
                return (pedido, "Pedido obtido com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
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
                var pedidos = PedidoAlimentoServico.ListarPedidos();
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
                var total = PedidoAlimentoServico.CalcularTotal(pedidoId);
                return (total, $"Total calculado com sucesso: R$ {total:F2}");
            }
            catch (RecursoNaoEncontradoExcecao ex)
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

        // CHECK-IN - registrar presenca do cliente
        public (bool sucesso, string mensagem) RealizarCheckIn(int ingressoId)
        {
            try
            {
                IngressoServico.RealizarCheckIn(ingressoId);
                return (true, "Check-in realizado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso nao encontrado: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operacao nao permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao realizar check-in.");
            }
        }

        private string? ConcederCortesiaPreEstreia(Sessao sessao, Cliente cliente)
        {
            if (sessao == null || cliente == null)
            {
                return null;
            }

            if (sessao.Tipo != TipoSessao.PreEstreia)
            {
                return null;
            }

            var cortesia = ProdutoAlimentoServico.ObterCortesiaPreEstreiaDisponivel();
            if (cortesia == null)
            {
                return "Sem cortesia disponivel para pre-estreia.";
            }

            ProdutoAlimentoServico.ReduzirEstoque(cortesia.Id, 1);
            cliente.Cortesias.Add(cortesia);
            return $"Cortesia liberada: {cortesia.Nome}";
        }
    }
}





