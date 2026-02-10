using cinecore.Data;
using cinecore.Models;
using cinecore.Enums;
using cinecore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Services
{
    /// <summary>
    /// Serviço de lógica de negócio para Aluguel de Salas
    /// </summary>
    public class AluguelSalaServico
    {
        private readonly CineFlowContext _context;
        private readonly SessaoServico _sessaoServico;
        
        private const decimal ValorHoraNormal = 200m;
        private const decimal ValorHoraXD = 350m;
        private const decimal ValorHoraVIP = 600m;
        private const decimal ValorHora4D = 450m;
        private const decimal ValorPacoteAniversario = 300m;

        public AluguelSalaServico(CineFlowContext context, SessaoServico sessaoServico)
        {
            _context = context;
            _sessaoServico = sessaoServico;
        }

        /// <summary>
        /// Solicita um novo aluguel de sala
        /// </summary>
        public void SolicitarAluguel(AluguelSala aluguel)
        {
            if (aluguel == null)
            {
                throw new DadosInvalidosExcecao("Aluguel nao pode ser nulo.");
            }

            if (aluguel.Sala == null)
            {
                throw new DadosInvalidosExcecao("Sala e obrigatoria.");
            }

            if (aluguel.Inicio == default || aluguel.Fim == default || aluguel.Fim <= aluguel.Inicio)
            {
                throw new DadosInvalidosExcecao("Periodo do aluguel invalido.");
            }

            if (string.IsNullOrWhiteSpace(aluguel.NomeCliente) || string.IsNullOrWhiteSpace(aluguel.Contato))
            {
                throw new DadosInvalidosExcecao("Nome e contato do cliente sao obrigatorios.");
            }

            if (ExisteConflitoAluguel(aluguel.Sala, aluguel.Inicio, aluguel.Fim))
            {
                throw new OperacaoNaoPermitidaExcecao("Sala ja possui aluguel nesse periodo.");
            }

            if (ExisteConflitoSessao(aluguel.Sala, aluguel.Inicio, aluguel.Fim))
            {
                throw new OperacaoNaoPermitidaExcecao("Sala possui sessao programada nesse periodo.");
            }

            if (aluguel.Valor <= 0m)
            {
                aluguel.Valor = CalcularValorAluguel(aluguel.Sala, aluguel.Inicio, aluguel.Fim, aluguel.PacoteAniversario);
            }

            aluguel.Status = StatusAluguel.Solicitado;
            aluguel.DataCriacao = DateTime.Now;
            _context.AlugueisSala.Add(aluguel);
            _context.SaveChanges();
        }

        /// <summary>
        /// Obtém um aluguel pelo ID
        /// </summary>
        public AluguelSala ObterAluguel(int id)
        {
            var aluguel = _context.AlugueisSala
                .Include(a => a.Sala)
                .Include(a => a.Cliente)
                .FirstOrDefault(a => a.Id == id);
            if (aluguel == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Aluguel com ID {id} nao encontrado.");
            }
            return aluguel;
        }

        /// <summary>
        /// Lista todos os aluguéis
        /// </summary>
        public List<AluguelSala> ListarAlugueis()
        {
            return _context.AlugueisSala
                .Include(a => a.Sala)
                .Include(a => a.Cliente)
                .ToList();
        }

        /// <summary>
        /// Lista aluguéis por status
        /// </summary>
        public List<AluguelSala> ListarPorStatus(StatusAluguel status)
        {
            return _context.AlugueisSala
                .Include(a => a.Sala)
                .Include(a => a.Cliente)
                .Where(a => a.Status == status)
                .ToList();
        }

        /// <summary>
        /// Lista aluguéis por sala
        /// </summary>
        public List<AluguelSala> ListarPorSala(int salaId)
        {
            return _context.AlugueisSala
                .Include(a => a.Sala)
                .Include(a => a.Cliente)
                .Where(a => a.Sala != null && a.Sala.Id == salaId)
                .ToList();
        }

        /// <summary>
        /// Aprova um aluguel e opcionalmente ajusta o valor
        /// </summary>
        public void AprovarAluguel(int id, decimal? valor = null)
        {
            var aluguel = ObterAluguel(id);
            
            if (aluguel.Status == StatusAluguel.Cancelado)
            {
                throw new OperacaoNaoPermitidaExcecao("Aluguel cancelado nao pode ser aprovado.");
            }
            
            if (valor.HasValue)
            {
                if (valor.Value < 0)
                {
                    throw new DadosInvalidosExcecao("Valor do aluguel invalido.");
                }
                aluguel.Valor = valor.Value;
            }
            else if (aluguel.Valor <= 0m)
            {
                if (aluguel.Sala != null)
                {
                    aluguel.Valor = CalcularValorAluguel(aluguel.Sala, aluguel.Inicio, aluguel.Fim, aluguel.PacoteAniversario);
                }
            }
            
            aluguel.Status = StatusAluguel.Aprovado;
            aluguel.DataAtualizacao = DateTime.Now;

            _context.SaveChanges();
        }

        /// <summary>
        /// Cancela um aluguel (apenas com 24h de antecedência)
        /// </summary>
        public void CancelarAluguel(int id)
        {
            var aluguel = ObterAluguel(id);
            
            if (aluguel.Inicio <= DateTime.Now.AddHours(24))
            {
                throw new OperacaoNaoPermitidaExcecao("Cancelamento permitido apenas com 24 horas de antecedencia.");
            }
            
            aluguel.Status = StatusAluguel.Cancelado;
            aluguel.DataAtualizacao = DateTime.Now;

            _context.SaveChanges();
        }

        /// <summary>
        /// Atualiza um aluguel
        /// </summary>
        public void AtualizarAluguel(int id, string? nomeCliente = null, string? contato = null, 
            string? motivo = null, DateTime? inicio = null, DateTime? fim = null, bool? pacoteAniversario = null)
        {
            var aluguel = ObterAluguel(id);

            if (!string.IsNullOrWhiteSpace(nomeCliente))
            {
                aluguel.NomeCliente = nomeCliente;
            }

            if (!string.IsNullOrWhiteSpace(contato))
            {
                aluguel.Contato = contato;
            }

            if (!string.IsNullOrWhiteSpace(motivo))
            {
                aluguel.Motivo = motivo;
            }

            if (pacoteAniversario.HasValue)
            {
                aluguel.PacoteAniversario = pacoteAniversario.Value;
            }

            if (inicio.HasValue && fim.HasValue)
            {
                if (fim <= inicio)
                {
                    throw new DadosInvalidosExcecao("Periodo do aluguel invalido.");
                }

                if (aluguel.Sala != null && ExisteConflitoAluguel(aluguel.Sala, inicio.Value, fim.Value, aluguel.Id))
                {
                    throw new OperacaoNaoPermitidaExcecao("Sala ja possui aluguel nesse periodo.");
                }

                if (aluguel.Sala != null && ExisteConflitoSessao(aluguel.Sala, inicio.Value, fim.Value))
                {
                    throw new OperacaoNaoPermitidaExcecao("Sala possui sessao programada nesse periodo.");
                }

                aluguel.Inicio = inicio.Value;
                aluguel.Fim = fim.Value;
            }

            aluguel.DataAtualizacao = DateTime.Now;

            _context.SaveChanges();
        }

        /// <summary>
        /// Deleta um aluguel
        /// </summary>
        public void DeletarAluguel(int id)
        {
            var aluguel = ObterAluguel(id);
            _context.AlugueisSala.Remove(aluguel);
            _context.SaveChanges();
        }

        // ===== MÉTODOS PRIVADOS =====

        private bool ExisteConflitoAluguel(Sala sala, DateTime inicio, DateTime fim, int? idParaIgnorar = null)
        {
            return _context.AlugueisSala.Any(a => 
                (!idParaIgnorar.HasValue || a.Id != idParaIgnorar.Value) &&
                a.Sala != null &&
                a.Sala.Id == sala.Id &&
                inicio < a.Fim && 
                fim > a.Inicio && 
                a.Status != StatusAluguel.Cancelado);
        }

        private bool ExisteConflitoSessao(Sala sala, DateTime inicio, DateTime fim)
        {
            var sessoes = _sessaoServico.ListarSessoesPorSala(sala.Id);
            return sessoes.Any(s =>
                inicio < s.DataHorario.AddMinutes(s.Filme?.Duracao ?? 0) &&
                fim > s.DataHorario);
        }

        private static decimal CalcularValorAluguel(Sala sala, DateTime inicio, DateTime fim, bool pacoteAniversario)
        {
            if (sala == null)
            {
                return 0m;
            }

            var duracao = fim - inicio;
            var horas = (int)Math.Ceiling(duracao.TotalHours);
            if (horas < 1)
            {
                horas = 1;
            }

            var valorHora = sala.Tipo == TipoSala.XD ? ValorHoraXD :
                sala.Tipo == TipoSala.VIP ? ValorHoraVIP :
                sala.Tipo == TipoSala.QuatroD ? ValorHora4D : ValorHoraNormal;

            var total = valorHora * horas;
            if (pacoteAniversario)
            {
                total += ValorPacoteAniversario;
            }

            return total;
        }
    }
}
