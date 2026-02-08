using cineflow.modelos;
using cineflow.excecoes;
using cineflow.enumeracoes;

namespace cineflow.servicos
{
    public class SessaoServico
    {
        private readonly List<Sessao> sessoes;
        private const float AdicionalSalaXD = 12f;
        private const float AdicionalSalaVIP = 35f;
        private const float AdicionalSala4D = 25f;
        private const float AdicionalFilme3D = 7f;
        private const float AdicionalPreEstreia = 15f;
        private const float AdicionalEvento = 10f;
        private const float DescontoEspecialBebe = 0.20f;
        private const float DescontoEspecialPet = 0.15f;
        private const float DescontoMatine = 0.25f;

        public SessaoServico()
        {
            sessoes = new List<Sessao>();
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

            sessao.Id = sessoes.Count > 0 ? sessoes.Max(s => s.Id) + 1 : 1;
            sessoes.Add(sessao);

            if (sessao.Filme.Sessoes != null && !sessao.Filme.Sessoes.Contains(sessao))
            {
                sessao.Filme.Sessoes.Add(sessao);
            }
        }

        public Sessao ObterSessao(int id)
        {
            var sessao = sessoes.FirstOrDefault(s => s.Id == id);
            if (sessao == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Sessão com ID {id} não encontrada.");
            }
            return sessao;
        }

        public List<Sessao> ListarSessoes()
        {
            return new List<Sessao>(sessoes);
        }

        public List<Sessao> ListarSessoesPorFilme(int filmeId)
        {
            return sessoes.Where(s => s.Filme?.Id == filmeId).ToList();
        }

        public List<Sessao> ListarSessoesPorSala(int salaId)
        {
            return sessoes.Where(s => s.Sala?.Id == salaId).ToList();
        }

        public void AtualizarSessao(int id, DateTime? dataHorario = null, float? precoBase = null, Filme? filme = null,
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
            if (sessoes.Any(s =>
                s.Id != sessao.Id &&
                s.Sala?.Id == novaSala.Id &&
                novoDataHorario < s.DataHorario.AddMinutes(s.Filme.Duracao) &&
                novoDataHorario.AddMinutes(novoFilme.Duracao) > s.DataHorario))
            {
                throw new OperacaoNaoPermitidaExcecao($"Conflito de horario na sala '{novaSala.Nome}'. Ja existe uma sessao neste horario.");
            }

            // Se mudou o filme, atualiza relacionamento
            if (sessao.Filme?.Id != novoFilme.Id)
            {
                if (sessao.Filme?.Sessoes != null)
                {
                    sessao.Filme.Sessoes.Remove(sessao);
                }
                if (novoFilme.Sessoes != null && !novoFilme.Sessoes.Contains(sessao))
                {
                    novoFilme.Sessoes.Add(sessao);
                }
            }

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
        }

        public void DeletarSessao(int id)
        {
            var sessao = ObterSessao(id);
            
            if (sessao.Filme?.Sessoes != null)
            {
                sessao.Filme.Sessoes.Remove(sessao);
            }
            
            sessoes.Remove(sessao);
        }

        // Validação de conflito de horários na mesma sala
        private bool ExisteConflito(DateTime inicio, DateTime fim, Sala sala)
        {
            return sessoes.Any(s =>
                s.Sala?.Id == sala.Id &&
                inicio < s.DataHorario.AddMinutes(s.Filme.Duracao) &&
                fim > s.DataHorario);
        }

        private static float CalcularAdicionalSala(TipoSala tipoSala)
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

            return 0f;
        }

        private static float CalcularAdicionalTipoSessao(TipoSessao tipoSessao)
        {
            if (tipoSessao == TipoSessao.PreEstreia)
            {
                return AdicionalPreEstreia;
            }

            if (tipoSessao == TipoSessao.Evento)
            {
                return AdicionalEvento;
            }

            return 0f;
        }

        private static float AplicarDescontos(float precoAtual, Sessao sessao)
        {
            float desconto = 0f;
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

            if (desconto <= 0f)
            {
                return precoAtual;
            }

            var precoComDesconto = precoAtual - (precoAtual * desconto);
            return Math.Max(0f, precoComDesconto);
        }

        private static float CalcularAdicional3D(bool eh3D)
        {
            return eh3D ? AdicionalFilme3D : 0f;
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




