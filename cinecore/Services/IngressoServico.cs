using cinecore.Data;
using cinecore.Models;
using cinecore.Enums;
using cinecore.Exceptions;
using cinecore.Utilities;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Services
{
    public class IngressoServico
    {
        private readonly CineFlowContext _context;
        // Casal cobra mais por 2 lugares; PCD sem adicional.
        private const decimal AdicionalAssentoCasal = 10m;
        private const decimal AdicionalAssentoPCD = 0m;
        private const decimal AdicionalAssentoPreferencial = 3m;
        private const decimal DescontoCupomParceiro = 0.10m;
        private const decimal TaxaReservaAntecipada = 5m;
        private const decimal ValorPorPonto = 0.10m;

        public IngressoServico(CineFlowContext context)
        {
            _context = context;
        }

        // VENDA - inteira com pagamento
        public Ingresso VenderInteira(Sessao sessao, Cliente cliente, char fila, int numero, FormaPagamento formaPagamento,
            decimal valorPago = 0m, string? cupomParceiro = null, bool reservaAntecipada = false, int pontosUsados = 0)
        {
            var assento = ValidarVenda(sessao, cliente, fila, numero);

            var precoFinalCompra = sessao.PrecoFinal + CalcularAdicionalAssento(assento);
            precoFinalCompra = AplicarDescontoAniversario(precoFinalCompra, cliente, sessao.Sala?.Tipo ?? TipoSala.Normal);
            precoFinalCompra = AplicarDescontoCupom(precoFinalCompra, sessao, cupomParceiro);
            precoFinalCompra = AplicarDescontoPontos(precoFinalCompra, cliente, pontosUsados);
            var taxaReserva = 0m;
            if (reservaAntecipada)
            {
                ValidarReservaAntecipada(sessao);
                taxaReserva = TaxaReservaAntecipada;
                precoFinalCompra += taxaReserva;
            }
            var ingresso = new IngressoInteira(precoFinalCompra, 0, fila, numero, DateTime.Now, 
                sessao, cliente, assento);
            AtualizarDadosFidelidade(ingresso, cliente, pontosUsados, taxaReserva, reservaAntecipada);

            RegistrarPagamento(ingresso, formaPagamento, valorPago);
            RegistrarVenda(ingresso, assento, sessao, cliente);
            _context.SaveChanges();
            return ingresso;
        }

        // VENDA - meia com pagamento
        public Ingresso VenderMeia(Sessao sessao, Cliente cliente, char fila, int numero, string motivo, FormaPagamento formaPagamento,
            decimal valorPago = 0m, string? cupomParceiro = null, bool reservaAntecipada = false, int pontosUsados = 0)
        {
            if (string.IsNullOrWhiteSpace(motivo))
            {
                throw new DadosInvalidosExcecao("O motivo para meia entrada é obrigatório.");
            }

            var assento = ValidarVenda(sessao, cliente, fila, numero);

            var precoFinalCompra = sessao.PrecoFinal + CalcularAdicionalAssento(assento);
            precoFinalCompra = AplicarDescontoAniversario(precoFinalCompra, cliente, sessao.Sala?.Tipo ?? TipoSala.Normal);
            precoFinalCompra = AplicarDescontoCupom(precoFinalCompra, sessao, cupomParceiro);
            precoFinalCompra = AplicarDescontoPontos(precoFinalCompra, cliente, pontosUsados);
            var taxaReserva = 0m;
            if (reservaAntecipada)
            {
                ValidarReservaAntecipada(sessao);
                taxaReserva = TaxaReservaAntecipada;
                precoFinalCompra += taxaReserva;
            }
            var ingresso = new IngressoMeia(precoFinalCompra, 0, fila, numero, DateTime.Now, motivo)
            {
                Motivo = motivo,
                Sessao = sessao,
                Cliente = cliente,
                Assento = assento
            };
            AtualizarDadosFidelidade(ingresso, cliente, pontosUsados, taxaReserva, reservaAntecipada);

            RegistrarPagamento(ingresso, formaPagamento, valorPago);
            RegistrarVenda(ingresso, assento, sessao, cliente);
            _context.SaveChanges();
            return ingresso;
        }

        public Ingresso ObterIngresso(int id)
        {
            var ingresso = _context.Ingressos
                .Include(i => i.Sessao!)
                    .ThenInclude(s => s.Sala!)
                .Include(i => i.Sessao!)
                    .ThenInclude(s => s.Filme!)
                .Include(i => i.Sessao!)
                    .ThenInclude(s => s.Ingressos!)
                .Include(i => i.Cliente!)
                    .ThenInclude(c => c.Ingressos!)
                .Include(i => i.Assento)
                .FirstOrDefault(i => i.Id == id);
            if (ingresso == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Ingresso com ID {id} não encontrado.");
            }
            return ingresso;
        }

        public List<Ingresso> ListarIngressos()
        {
            return _context.Ingressos
                .Include(i => i.Sessao!)
                    .ThenInclude(s => s.Filme!)
                .Include(i => i.Sessao!)
                    .ThenInclude(s => s.Sala!)
                .Include(i => i.Cliente!)
                    .ThenInclude(c => c.Ingressos!)
                .Include(i => i.Assento)
                .ToList();
        }

        public void CancelarIngresso(int id)
        {
            var ingresso = ObterIngresso(id);
            if (ingresso == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Ingresso com ID {id} não encontrado.");
            }

            if (ingresso.Sessao?.DataHorario <= DateTime.Now.AddHours(24))
            {
                throw new OperacaoNaoPermitidaExcecao("Cancelamento permitido apenas com 24 horas de antecedencia.");
            }

            if (ingresso.Assento != null)
            {
                ingresso.Assento.Liberar();
                ingresso.Assento.Ingresso = null;
            }

            if (ingresso.Sessao != null)
            {
                ingresso.Sessao.Ingressos.Remove(ingresso);
            }

            if (ingresso.Cliente != null)
            {
                ingresso.Cliente.Ingressos.Remove(ingresso);
            }

            _context.Ingressos.Remove(ingresso);
            _context.SaveChanges();
        }

        // CHECK-IN - registrar presenca do cliente
        public void RealizarCheckIn(int ingressoId)
        {
            var ingresso = ObterIngresso(ingressoId);
            if (ingresso == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Ingresso com ID {ingressoId} não encontrado.");
            }

            if (ingresso.CheckInRealizado)
            {
                throw new OperacaoNaoPermitidaExcecao("Check-in ja realizado.");
            }

            if (ingresso.Sessao != null && DateTime.Now > ingresso.Sessao.DataHorario)
            {
                throw new OperacaoNaoPermitidaExcecao("Check-in indisponivel para sessao encerrada.");
            }

            ingresso.CheckInRealizado = true;
            ingresso.DataCheckIn = DateTime.Now;
            // Bonus simples de fidelidade por check-in.
            if (ingresso.Cliente != null)
            {
                ingresso.Cliente.AdicionarPontos(5);
            }

            _context.SaveChanges();
        }

        private Assento ValidarVenda(Sessao sessao, Cliente cliente, char fila, int numero)
        {
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
            if (sessao!.Sala == null)
            {
                throw new ErroOperacaoCriticaExcecao("Sessão sem sala associada.");
            }

            if (sessao.Filme == null)
            {
                throw new ErroOperacaoCriticaExcecao("Sessão sem filme associado.");
            }

            if (sessao.PrecoFinal < 0)
            {
                throw new ErroOperacaoCriticaExcecao("Preco invalido.");
            }

            int idadeCliente = cliente!.ObterIdade(DateTime.Now);
            int classificacao = (int)sessao.Filme.Classificacao;
            if (idadeCliente < classificacao)
            {
                throw new OperacaoNaoPermitidaExcecao("Cliente nao possui idade minima para este filme.");
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

            if (sessao != null && !sessao.Ingressos.Contains(ingresso))
            {
                sessao.Ingressos.Add(ingresso);
            }

            if (cliente != null && !cliente.Ingressos.Contains(ingresso))
            {
                cliente.Ingressos.Add(ingresso);
            }

            _context.Ingressos.Add(ingresso);
        }

        // Registra forma de pagamento e calcula troco para dinheiro.
        private static void RegistrarPagamento(Ingresso ingresso, FormaPagamento formaPagamento, decimal valorPago)
        {
            var total = Math.Round(ingresso.CalcularPreco(0m), 2);

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
        private static decimal CalcularAdicionalAssento(Assento assento)
        {
            decimal adicional = 0m;

            if (assento.Tipo == TipoAssento.Casal)
            {
                adicional += AdicionalAssentoCasal;
            }

            if (assento.Tipo == TipoAssento.PCD)
            {
                adicional += AdicionalAssentoPCD;
            }

            if (assento.Preferencial)
            {
                adicional += AdicionalAssentoPreferencial;
            }

            return adicional;
        }

        private static decimal AplicarDescontoAniversario(decimal preco, Cliente cliente, TipoSala tipoSala)
        {
            if (cliente == null || !cliente.EhMesAniversario(DateTime.Now))
            {
                return preco;
            }

            decimal desconto = 0m;
            if (tipoSala == TipoSala.Normal)
            {
                desconto = 1.0m;
            }
            else if (tipoSala == TipoSala.XD)
            {
                desconto = 0.50m;
            }
            else
            {
                desconto = 0.25m;
            }

            var precoComDesconto = preco - (preco * desconto);
            return Math.Max(0m, precoComDesconto);
        }

        private static decimal AplicarDescontoCupom(decimal preco, Sessao sessao, string? cupomParceiro)
        {
            if (sessao == null || string.IsNullOrWhiteSpace(sessao.Parceiro))
            {
                return preco;
            }

            if (string.IsNullOrWhiteSpace(cupomParceiro))
            {
                return preco;
            }

            if (!cupomParceiro.Equals(sessao.Parceiro, StringComparison.OrdinalIgnoreCase))
            {
                return preco;
            }

            var precoComDesconto = preco - (preco * DescontoCupomParceiro);
            return Math.Max(0m, precoComDesconto);
        }

        private static decimal AplicarDescontoPontos(decimal preco, Cliente cliente, int pontosUsados)
        {
            if (cliente == null || pontosUsados <= 0)
            {
                return preco;
            }

            if (!cliente.TentarUsarPontos(pontosUsados))
            {
                throw new OperacaoNaoPermitidaExcecao("Pontos insuficientes.");
            }

            var desconto = (decimal)pontosUsados * ValorPorPonto;
            var precoComDesconto = preco - desconto;
            return Math.Max(0m, precoComDesconto);
        }

        private static void ValidarReservaAntecipada(Sessao sessao)
        {
            if (sessao == null)
            {
                return;
            }

            if (sessao.DataHorario <= DateTime.Now.AddHours(1))
            {
                throw new OperacaoNaoPermitidaExcecao("Reserva antecipada deve ser feita com pelo menos 1 hora de antecedencia.");
            }
        }

        private static void AtualizarDadosFidelidade(Ingresso ingresso, Cliente cliente, int pontosUsados, decimal taxaReserva, bool reservaAntecipada)
        {
            if (ingresso == null || cliente == null)
            {
                return;
            }

            ingresso.ReservaAntecipada = reservaAntecipada;
            ingresso.TaxaReserva = taxaReserva;
            ingresso.PontosUsados = pontosUsados;

            var pontosGerados = (int)Math.Floor((double)ingresso.CalcularPreco(0m));
            ingresso.PontosGerados = pontosGerados;
            cliente.AdicionarPontos(pontosGerados);
        }
    }
}
