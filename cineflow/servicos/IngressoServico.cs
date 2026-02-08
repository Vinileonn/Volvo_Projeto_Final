using cineflow.modelos;
using cineflow.modelos.IngressoModelo;
using cineflow.modelos.UsuarioModelo;
using cineflow.enumeracoes;
using cineflow.excecoes;
using cineflow.utilitarios;

namespace cineflow.servicos
{
    public class IngressoServico
    {
        private readonly List<Ingresso> ingressos;
        // Casal cobra mais por 2 lugares; PCD sem adicional.
        private const float AdicionalAssentoCasal = 10f;
        private const float AdicionalAssentoPCD = 0f;

        public IngressoServico()
        {
            ingressos = new List<Ingresso>();
        }

        // VENDA - inteira com pagamento
        public Ingresso VenderInteira(Sessao sessao, Cliente cliente, char fila, int numero, FormaPagamento formaPagamento, decimal valorPago = 0m)
        {
            var assento = ValidarVenda(sessao, cliente, fila, numero);

            var precoFinalCompra = sessao.PrecoFinal + CalcularAdicionalAssento(assento);
            var ingresso = new IngressoInteira(precoFinalCompra, ProximoId(), fila, numero, sessao, cliente, assento, DateTime.Now);

            RegistrarPagamento(ingresso, formaPagamento, valorPago);
            RegistrarVenda(ingresso, assento, sessao, cliente);
            return ingresso;
        }

        // VENDA - meia com pagamento
        public Ingresso VenderMeia(Sessao sessao, Cliente cliente, char fila, int numero, string motivo, FormaPagamento formaPagamento, decimal valorPago = 0m)
        {
            if (string.IsNullOrWhiteSpace(motivo))
            {
                throw new DadosInvalidosExcecao("O motivo para meia entrada é obrigatório.");
            }

            var assento = ValidarVenda(sessao, cliente, fila, numero);

            var precoFinalCompra = sessao.PrecoFinal + CalcularAdicionalAssento(assento);
            var ingresso = new IngressoMeia(precoFinalCompra, ProximoId(), fila, numero, sessao, cliente, assento, DateTime.Now, motivo);

            RegistrarPagamento(ingresso, formaPagamento, valorPago);
            RegistrarVenda(ingresso, assento, sessao, cliente);
            return ingresso;
        }

        public Ingresso ObterIngresso(int id)
        {
            var ingresso = ingressos.FirstOrDefault(i => i.Id == id);
            if (ingresso == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Ingresso com ID {id} não encontrado.");
            }
            return ingresso;
        }

        public List<Ingresso> ListarIngressos()
        {
            return new List<Ingresso>(ingressos);
        }

        public void CancelarIngresso(int id)
        {
            var ingresso = ObterIngresso(id);
            if (ingresso == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Ingresso com ID {id} não encontrado.");
            }

            ingresso.Assento.Liberar();
            ingresso.Assento.Ingresso = null;
            ingresso.Sessao.Ingressos.Remove(ingresso);
            ingresso.Cliente.Ingressos.Remove(ingresso);

            ingressos.Remove(ingresso);
        }

        private Assento ValidarVenda(Sessao sessao, Cliente cliente, char fila, int numero)
        {
            if (sessao == null)
            {
                throw new DadosInvalidosExcecao("Sessão nula.");
            }
            // Validações básicas agrupadas
            var erros = new List<string>();
            
            if (sessao == null)
                erros.Add("sessão nula");
            
            if (cliente == null)
                erros.Add("cliente nulo");
            
            if (erros.Count > 0)
            {
                throw new DadosInvalidosExcecao($"Dados inválidos: {string.Join(", ", erros)}.");
            }

            // Validações críticas da sessão
            if (sessao?.Sala == null)
            {
                throw new ErroOperacaoCriticaExcecao("Sessão sem sala associada.");
            }

            if (sessao.PrecoFinal < 0)
            {
                throw new ErroOperacaoCriticaExcecao("Preco invalido.");
            }

            // Validações de localização do assento (fila e número)
            var errosAssento = new List<string>();
            if (!char.IsLetter(fila))
                errosAssento.Add("fila inválida");
            
            if (numero <= 0)
                errosAssento.Add("número inválido");
            
            if (errosAssento.Count > 0)
            {
                throw new DadosInvalidosExcecao($"Localização do assento inválida: {string.Join(", ", errosAssento)}.");
            }

            var assentoSala = sessao.Sala.ConsultarAssento(fila, numero);
            if (assentoSala == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Assento {fila}{numero} não existe.");
            }

            if (!assentoSala.Disponivel)
            {
                throw new OperacaoNaoPermitidaExcecao($"Assento {fila}{numero} indisponível.");
            }

            return assentoSala;
        }

        private void RegistrarVenda(Ingresso ingresso, Assento assento, Sessao sessao, Cliente cliente)
        {
            assento.Reservar();
            assento.Ingresso = ingresso;

            if (!sessao.Ingressos.Contains(ingresso))
            {
                sessao.Ingressos.Add(ingresso);
            }

            if (!cliente.Ingressos.Contains(ingresso))
            {
                cliente.Ingressos.Add(ingresso);
            }

            ingressos.Add(ingresso);
        }

        private int ProximoId()
        {
            return ingressos.Count > 0 ? ingressos.Max(i => i.Id) + 1 : 1;
        }

        // Registra forma de pagamento e calcula troco para dinheiro.
        private static void RegistrarPagamento(Ingresso ingresso, FormaPagamento formaPagamento, decimal valorPago)
        {
            var total = Math.Round((decimal)ingresso.CalcularPreco(0f), 2);

            ingresso.FormaPagamento = formaPagamento;
            ingresso.TrocoDetalhado = new Dictionary<decimal, int>();
            ingresso.ValorTroco = 0m;

            if (formaPagamento == FormaPagamento.Dinheiro)
            {
                if (valorPago < total)
                {
                    throw new DadosInvalidosExcecao("Valor pago menor que o total.");
                }

                ingresso.ValorPago = valorPago;
                ingresso.TrocoDetalhado = CalculadoraTroco.CalcularTroco(total, valorPago, out var troco);
                ingresso.ValorTroco = troco;
                return;
            }

            // Para outras formas, considera pagamento exato.
            ingresso.ValorPago = total;
        }

        // Aplica adicional por tipo de assento (casal cobra mais por 2 lugares, PCD sem adicional).
        private static float CalcularAdicionalAssento(Assento assento)
        {
            if (assento.Tipo == TipoAssento.Casal)
            {
                return AdicionalAssentoCasal;
            }

            if (assento.Tipo == TipoAssento.PCD)
            {
                return AdicionalAssentoPCD;
            }

            return 0f;
        }
    }
}




