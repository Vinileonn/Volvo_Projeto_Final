using cinecore.Data;
using cinecore.Models;
using cinecore.Exceptions;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Services
{
    public class SessaoServico
    {
        private readonly CineFlowContext _context;
        private const decimal AdicionalSalaXD = 12m;
        private const decimal AdicionalSalaVIP = 35m;
        private const decimal AdicionalSala4D = 25m;
        private const decimal AdicionalFilme3D = 7m;
        private const decimal AdicionalPreEstreia = 15m;
        private const decimal AdicionalEvento = 10m;
        private const decimal DescontoEspecialBebe = 0.20m;
        private const decimal DescontoEspecialPet = 0.15m;
        private const decimal DescontoMatine = 0.25m;

        public SessaoServico(CineFlowContext context)
        {
            _context = context;
        }

        public void CriarSessao(Sessao sessao)
        {
            if (sessao == null)
            {
                throw new DadosInvalidosExcecao("Sessao nao pode ser nula.");
            }

            if (sessao.DataHorario == default)
            {
                throw new DadosInvalidosExcecao("Data e horario sao obrigatorios.");
            }

            if (sessao.PrecoBase < 0)
            {
                throw new DadosInvalidosExcecao("Preco base nao pode ser negativo.");
            }

            if (sessao.Filme == null)
            {
                throw new DadosInvalidosExcecao("Filme e obrigatorio.");
            }

            if (sessao.Sala == null)
            {
                throw new DadosInvalidosExcecao("Sala e obrigatoria.");
            }

            if (ExisteConflito(sessao.DataHorario, sessao.DataHorario.AddMinutes(sessao.Filme.Duracao), sessao.Sala))
            {
                throw new OperacaoNaoPermitidaExcecao($"Conflito de horario na sala '{sessao.Sala.Nome}'. Ja existe uma sessao neste horario.");
            }

            if (sessao.Tipo != TipoSessao.Evento)
            {
                sessao.NomeEvento = null;
                sessao.Parceiro = null;
            }

            ValidarRequisitos(sessao);

            var adicionalSala = CalcularAdicionalSala(sessao.Sala.Tipo);
            var adicional3D = CalcularAdicional3D(sessao.Filme.Eh3D);
            var adicionalTipoSessao = CalcularAdicionalTipoSessao(sessao.Tipo);
            sessao.RecalcularPreco(adicionalSala + adicionalTipoSessao, adicional3D);
            sessao.PrecoFinal = AplicarDescontos(sessao.PrecoFinal, sessao);

            _context.Sessoes.Add(sessao);
            _context.SaveChanges();
        }

        public Sessao ObterSessao(int id)
        {
            var sessao = _context.Sessoes
                .Include(s => s.Filme!)
                .Include(s => s.Sala!)
                    .ThenInclude(sala => sala.Assentos!)
                .Include(s => s.Ingressos!)
                .FirstOrDefault(s => s.Id == id);
            if (sessao == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Sessão com ID {id} não encontrada.");
            }
            return sessao;
        }

        public List<Sessao> ListarSessoes()
        {
            return _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                .Include(s => s.Ingressos)
                .ToList();
        }

        public List<Sessao> ListarSessoesPorFilme(int filmeId)
        {
            return _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                .Include(s => s.Ingressos)
                .Where(s => s.Filme != null && s.Filme.Id == filmeId)
                .ToList();
        }

        public List<Sessao> ListarSessoesPorSala(int salaId)
        {
            return _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                .Include(s => s.Ingressos)
                .Where(s => s.Sala != null && s.Sala.Id == salaId)
                .ToList();
        }

        public void AtualizarSessao(int id, DateTime? dataHorario = null, decimal? precoBase = null, Filme? filme = null,
            Sala? sala = null, TipoSessao? tipo = null, string? nomeEvento = null, string? parceiro = null,
            IdiomaSessao? idioma = null)
        {
            var sessao = ObterSessao(id);

            var novoDataHorario = dataHorario ?? sessao.DataHorario;
            var novoPrecoBase = precoBase ?? sessao.PrecoBase;
            var novoFilme = filme ?? sessao.Filme;
            var novaSala = sala ?? sessao.Sala;
            var novoTipo = tipo ?? sessao.Tipo;
            var novoNomeEvento = nomeEvento ?? sessao.NomeEvento;
            var novoParceiro = parceiro ?? sessao.Parceiro;
            var novoIdioma = idioma ?? sessao.Idioma;

            if (novoTipo != TipoSessao.Evento)
            {
                novoNomeEvento = null;
                novoParceiro = null;
            }

            if (novoDataHorario == default)
            {
                throw new DadosInvalidosExcecao("Data e horario invalidos.");
            }

            if (novoPrecoBase < 0)
            {
                throw new DadosInvalidosExcecao("Preco base nao pode ser negativo.");
            }

            if (novoFilme == null)
            {
                throw new DadosInvalidosExcecao("Filme nao pode ser nulo.");
            }

            if (novaSala == null)
            {
                throw new DadosInvalidosExcecao("Sala nao pode ser nula.");
            }

            // Validacao de conflito (excluindo a propria sessao)
            if (_context.Sessoes.Any(s =>
                s.Id != sessao.Id &&
                s.Sala != null &&
                s.Filme != null &&
                s.Sala.Id == novaSala.Id &&
                novoDataHorario < s.DataHorario.AddMinutes(s.Filme.Duracao) &&
                novoDataHorario.AddMinutes(novoFilme.Duracao) > s.DataHorario))
            {
                throw new OperacaoNaoPermitidaExcecao($"Conflito de horario na sala '{novaSala.Nome}'. Ja existe uma sessao neste horario.");
            }

            // Se mudou o filme, atualiza relacionamento
            sessao.DataHorario = novoDataHorario;
            sessao.PrecoBase = novoPrecoBase;
            sessao.Filme = novoFilme;
            sessao.Sala = novaSala;
            sessao.Tipo = novoTipo;
            sessao.NomeEvento = novoNomeEvento;
            sessao.Parceiro = novoParceiro;
            sessao.Idioma = novoIdioma;

            ValidarRequisitos(sessao);

            var adicionalSala = CalcularAdicionalSala(novaSala.Tipo);
            var adicional3D = CalcularAdicional3D(novoFilme.Eh3D);
            var adicionalTipoSessao = CalcularAdicionalTipoSessao(novoTipo);
            sessao.RecalcularPreco(adicionalSala + adicionalTipoSessao, adicional3D);
            sessao.PrecoFinal = AplicarDescontos(sessao.PrecoFinal, sessao);

            _context.SaveChanges();
        }

        public void DeletarSessao(int id)
        {
            var sessao = ObterSessao(id);
            _context.Sessoes.Remove(sessao);
            _context.SaveChanges();
        }

        // Validação de conflito de horários na mesma sala
        private bool ExisteConflito(DateTime inicio, DateTime fim, Sala sala)
        {
            return _context.Sessoes.Any(s =>
                s.Sala != null &&
                s.Filme != null &&
                s.Sala.Id == sala.Id &&
                inicio < s.DataHorario.AddMinutes(s.Filme.Duracao) &&
                fim > s.DataHorario);
        }

        private static decimal CalcularAdicionalSala(TipoSala tipoSala)
        {
            if (tipoSala == TipoSala.XD)
            {
                return AdicionalSalaXD;
            }

            if (tipoSala == TipoSala.VIP)
            {
                return AdicionalSalaVIP;
            }

            if (tipoSala == TipoSala.QuatroD)
            {
                return AdicionalSala4D;
            }

            return 0m;
        }

        private static decimal CalcularAdicionalTipoSessao(TipoSessao tipoSessao)
        {
            if (tipoSessao == TipoSessao.PreEstreia)
            {
                return AdicionalPreEstreia;
            }

            if (tipoSessao == TipoSessao.Evento)
            {
                return AdicionalEvento;
            }

            return 0m;
        }

        private static decimal AplicarDescontos(decimal precoAtual, Sessao sessao)
        {
            decimal desconto = 0m;
            if (sessao.Tipo == TipoSessao.EspecialBebe)
            {
                desconto += DescontoEspecialBebe;
            }
            else if (sessao.Tipo == TipoSessao.EspecialPet)
            {
                desconto += DescontoEspecialPet;
            }
            else if (sessao.Tipo == TipoSessao.Matine)
            {
                desconto += DescontoMatine;
            }

            if (desconto <= 0m)
            {
                return precoAtual;
            }

            var precoComDesconto = precoAtual - (precoAtual * desconto);
            return Math.Max(0m, precoComDesconto);
        }

        private static decimal CalcularAdicional3D(bool eh3D)
        {
            return eh3D ? AdicionalFilme3D : 0m;
        }

        private static void ValidarRequisitos(Sessao sessao)
        {
            if (sessao.Sala?.Cinema == null)
            {
                return;
            }

            var funcionarios = sessao.Sala.Cinema.Funcionarios;
            if (sessao.Sala.Tipo == TipoSala.VIP && !funcionarios.Any(f => f.Cargo == CargoFuncionario.Garcom))
            {
                throw new OperacaoNaoPermitidaExcecao("Sala VIP exige ao menos um garcom no cinema.");
            }

            if (sessao.Tipo == TipoSessao.PreEstreia && !funcionarios.Any(f => f.Cargo == CargoFuncionario.Gerente))
            {
                throw new OperacaoNaoPermitidaExcecao("Pre-estreia exige gerente no cinema.");
            }
        }
    }
}
