using cineflow.modelos;
using cineflow.excecoes;
using cineflow.enumeracoes;

namespace cineflow.servicos
{
    // Renomeado de SessaoService para SessaoServico.
    public class SessaoServico
    {
        private readonly List<Sessao> sessoes;
        // ANTIGO:
        // private const float AdicionalSalaXD = 10f;
        // private const float AdicionalFilme3D = 5f;

        // Ajuste de valores para diferenciar melhor XD e 3D no preco final.
        private const float AdicionalSalaXD = 12f;
        private const float AdicionalFilme3D = 7f;

        public SessaoServico()
        {
            sessoes = new List<Sessao>();
        }
// ANTIGO: // CRIAR - 
// CRIAR - 
        // ANTIGO:
        // public void CriarSessao(Sessao sessao)
        // {
        //     if (sessao == null)
        //     {
        //         throw new DadosInvalidosExcecao("Sessão não pode ser nula.");
        //     }
        //
        //     if (sessao.DataHorario == default)
        //     {
        //         throw new DadosInvalidosExcecao("Data e horário são obrigatórios.");
        //     }
        //
        //     if (sessao.Preco < 0)
        //     {
        //         throw new DadosInvalidosExcecao("Preço não pode ser negativo.");
        //     }
        //
        //     if (sessao.Filme == null)
        //     {
        //         throw new DadosInvalidosExcecao("Filme é obrigatório.");
        //     }
        //
        //     if (sessao.Sala == null)
        //     {
        //         throw new DadosInvalidosExcecao("Sala é obrigatória.");
        //     }
        //
        //     if (ExisteConflito(sessao.DataHorario, sessao.DataHorario.AddMinutes(sessao.Filme.Duracao), sessao.Sala))
        //     {
        //         throw new OperacaoNaoPermitidaExcecao($"Conflito de horário na sala '{sessao.Sala.Nome}'. Já existe uma sessão neste horário.");
        //     }
        //
        //     sessao.Id = sessoes.Count > 0 ? sessoes.Max(s => s.Id) + 1 : 1;
        //     sessoes.Add(sessao);
        //
        //     if (sessao.Filme.Sessoes != null && !sessao.Filme.Sessoes.Contains(sessao))
        //     {
        //         sessao.Filme.Sessoes.Add(sessao);
        //     }
        // }

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

            var adicionalSala = CalcularAdicionalSala(sessao.Sala.Tipo);
            var adicional3D = CalcularAdicional3D(sessao.Filme.Eh3D);
            sessao.RecalcularPreco(adicionalSala, adicional3D);

            sessao.Id = sessoes.Count > 0 ? sessoes.Max(s => s.Id) + 1 : 1;
            sessoes.Add(sessao);

            if (sessao.Filme.Sessoes != null && !sessao.Filme.Sessoes.Contains(sessao))
            {
                sessao.Filme.Sessoes.Add(sessao);
            }
        }
// ANTIGO: // LER - 
// LER - 
        public Sessao ObterSessao(int id)
        {
            var sessao = sessoes.FirstOrDefault(s => s.Id == id);
            if (sessao == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Sessão com ID {id} não encontrada.");
            }
            return sessao;
        }
// ANTIGO: // LER - 
// LER - 
        public List<Sessao> ListarSessoes()
        {
            return new List<Sessao>(sessoes);
        }
// ANTIGO: // LER - 
// LER - 
        public List<Sessao> ListarSessoesPorFilme(int filmeId)
        {
            return sessoes.Where(s => s.Filme?.Id == filmeId).ToList();
        }
// ANTIGO: // LER - 
// LER - 
        public List<Sessao> ListarSessoesPorSala(int salaId)
        {
            return sessoes.Where(s => s.Sala?.Id == salaId).ToList();
        }
// ANTIGO: // ATUALIZAR - 
// ATUALIZAR - 
        // ANTIGO:
        // public void AtualizarSessao(int id, DateTime? dataHorario = null, float? preco = null, Filme? filme = null, Sala? sala = null)
        // {
        //     var sessao = ObterSessao(id);
        //
        //     var novoDataHorario = dataHorario ?? sessao.DataHorario;
        //     var novoPreco = preco ?? sessao.Preco;
        //     var novoFilme = filme ?? sessao.Filme;
        //     var novaSala = sala ?? sessao.Sala;
        //
        //     if (novoDataHorario == default)
        //     {
        //         throw new DadosInvalidosExcecao("Data e horário inválidos.");
        //     }
        //
        //     if (novoPreco < 0)
        //     {
        //         throw new DadosInvalidosExcecao("Preço não pode ser negativo.");
        //     }
        //
        //     if (novoFilme == null)
        //     {
        //         throw new DadosInvalidosExcecao("Filme não pode ser nulo.");
        //     }
        //
        //     if (novaSala == null)
        //     {
        //         throw new DadosInvalidosExcecao("Sala não pode ser nula.");
        //     }
        //
        //     // Validação de conflito (excluindo a própria sessão)
        //     if (sessoes.Any(s =>
        //         s.Id != sessao.Id &&
        //         s.Sala?.Id == novaSala.Id &&
        //         novoDataHorario < s.DataHorario.AddMinutes(s.Filme.Duracao) &&
        //         novoDataHorario.AddMinutes(novoFilme.Duracao) > s.DataHorario))
        //     {
        //         throw new OperacaoNaoPermitidaExcecao($"Conflito de horário na sala '{novaSala.Nome}'. Já existe uma sessão neste horário.");
        //     }
        //
        //     // Se mudou o filme, atualiza relacionamento
        //     if (sessao.Filme?.Id != novoFilme.Id)
        //     {
        //         if (sessao.Filme?.Sessoes != null)
        //         {
        //             sessao.Filme.Sessoes.Remove(sessao);
        //         }
        //         if (novoFilme.Sessoes != null && !novoFilme.Sessoes.Contains(sessao))
        //         {
        //             novoFilme.Sessoes.Add(sessao);
        //         }
        //     }
        //
        //     sessao.DataHorario = novoDataHorario;
        //     sessao.Preco = novoPreco;
        //     sessao.Filme = novoFilme;
        //     sessao.Sala = novaSala;
        // }

        public void AtualizarSessao(int id, DateTime? dataHorario = null, float? precoBase = null, Filme? filme = null, Sala? sala = null)
        {
            var sessao = ObterSessao(id);

            var novoDataHorario = dataHorario ?? sessao.DataHorario;
            var novoPrecoBase = precoBase ?? sessao.PrecoBase;
            var novoFilme = filme ?? sessao.Filme;
            var novaSala = sala ?? sessao.Sala;

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

            var adicionalSala = CalcularAdicionalSala(novaSala.Tipo);
            var adicional3D = CalcularAdicional3D(novoFilme.Eh3D);
            sessao.RecalcularPreco(adicionalSala, adicional3D);
        }
// ANTIGO: // EXCLUIR - 
// EXCLUIR - 
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
            return tipoSala == TipoSala.XD ? AdicionalSalaXD : 0f;
        }

        private static float CalcularAdicional3D(bool eh3D)
        {
            return eh3D ? AdicionalFilme3D : 0f;
        }
    }
}




