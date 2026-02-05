using cinema.models;
using cinema.exceptions;

namespace cinema.services
{
    public class SessaoService
    {
        private readonly List<Sessao> sessoes;

        public SessaoService()
        {
            sessoes = new List<Sessao>();
        }

        // CREATE - cadastrar nova sessão com validação de conflito
        public void CriarSessao(Sessao sessao)
        {
            if (sessao == null)
            {
                throw new DadosInvalidosException("Sessão não pode ser nula.");
            }

            if (sessao.DataHorario == default)
            {
                throw new DadosInvalidosException("Data e horário são obrigatórios.");
            }

            if (sessao.Preco < 0)
            {
                throw new DadosInvalidosException("Preço não pode ser negativo.");
            }

            if (sessao.Filme == null)
            {
                throw new DadosInvalidosException("Filme é obrigatório.");
            }

            if (sessao.Sala == null)
            {
                throw new DadosInvalidosException("Sala é obrigatória.");
            }

            if (ExisteConflito(sessao.DataHorario, sessao.DataHorario.AddMinutes(sessao.Filme.Duracao), sessao.Sala))
            {
                throw new OperacaoNaoPermitidaException($"Conflito de horário na sala '{sessao.Sala.Nome}'. Já existe uma sessão neste horário.");
            }

            sessao.Id = sessoes.Count > 0 ? sessoes.Max(s => s.Id) + 1 : 1;
            sessoes.Add(sessao);

            if (sessao.Filme.Sessoes != null && !sessao.Filme.Sessoes.Contains(sessao))
            {
                sessao.Filme.Sessoes.Add(sessao);
            }
        }

        // READ - obter sessão por id
        public Sessao ObterSessao(int id)
        {
            var sessao = sessoes.FirstOrDefault(s => s.Id == id);
            if (sessao == null)
            {
                throw new RecursoNaoEncontradoException($"Sessão com ID {id} não encontrada.");
            }
            return sessao;
        }

        // READ - listar todas as sessões
        public List<Sessao> ListarSessoes()
        {
            return new List<Sessao>(sessoes);
        }

        // READ - listar sessões por filme
        public List<Sessao> ListarSessoesPorFilme(int filmeId)
        {
            return sessoes.Where(s => s.Filme?.Id == filmeId).ToList();
        }

        // READ - listar sessões por sala
        public List<Sessao> ListarSessoesPorSala(int salaId)
        {
            return sessoes.Where(s => s.Sala?.Id == salaId).ToList();
        }

        // UPDATE - atualizar sessão com validação de conflito
        public void AtualizarSessao(int id, DateTime? dataHorario = null, float? preco = null, Filme? filme = null, Sala? sala = null)
        {
            var sessao = ObterSessao(id);

            var novoDataHorario = dataHorario ?? sessao.DataHorario;
            var novoPreco = preco ?? sessao.Preco;
            var novoFilme = filme ?? sessao.Filme;
            var novaSala = sala ?? sessao.Sala;

            if (novoDataHorario == default)
            {
                throw new DadosInvalidosException("Data e horário inválidos.");
            }

            if (novoPreco < 0)
            {
                throw new DadosInvalidosException("Preço não pode ser negativo.");
            }

            if (novoFilme == null)
            {
                throw new DadosInvalidosException("Filme não pode ser nulo.");
            }

            if (novaSala == null)
            {
                throw new DadosInvalidosException("Sala não pode ser nula.");
            }

            // Validação de conflito (excluindo a própria sessão)
            if (sessoes.Any(s =>
                s.Id != sessao.Id &&
                s.Sala?.Id == novaSala.Id &&
                novoDataHorario < s.DataHorario.AddMinutes(s.Filme.Duracao) &&
                novoDataHorario.AddMinutes(novoFilme.Duracao) > s.DataHorario))
            {
                throw new OperacaoNaoPermitidaException($"Conflito de horário na sala '{novaSala.Nome}'. Já existe uma sessão neste horário.");
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
            sessao.Preco = novoPreco;
            sessao.Filme = novoFilme;
            sessao.Sala = novaSala;
        }

        // DELETE - remover sessão
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
    }
}