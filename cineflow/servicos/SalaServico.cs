using cineflow.modelos;
using cineflow.utilitarios;
using cineflow.excecoes;

namespace cineflow.servicos
{
    public class SalaServico
    {
        private readonly List<Sala> salas;

        public SalaServico()
        {
            salas = new List<Sala>();
        }

        public void CriarSala(Sala sala)
        {
            if (sala == null)
            {
                throw new DadosInvalidosExcecao("Sala nao pode ser nula.");
            }

            if (string.IsNullOrWhiteSpace(sala.Nome))
            {
                throw new DadosInvalidosExcecao("Nome da sala e obrigatorio.");
            }

            if (sala.Capacidade <= 0)
            {
                throw new DadosInvalidosExcecao("Capacidade deve ser maior que zero.");
            }

            if (sala.Cinema == null)
            {
                throw new DadosInvalidosExcecao("Cinema e obrigatorio.");
            }

            if (sala.QuantidadeAssentosCasal < 0 || sala.QuantidadeAssentosPCD < 0)
            {
                throw new DadosInvalidosExcecao("Quantidade de assentos especiais invalida.");
            }

            int lugaresEspeciais = sala.QuantidadeAssentosPCD + (sala.QuantidadeAssentosCasal * 2);
            if (lugaresEspeciais > sala.Capacidade)
            {
                throw new DadosInvalidosExcecao("Quantidade de assentos especiais excede a capacidade.");
            }

            if (salas.Any(s =>
                s.Cinema != null &&
                s.Cinema.Id == sala.Cinema.Id &&
                s.Nome.Equals(sala.Nome, StringComparison.OrdinalIgnoreCase)))
            {
                throw new OperacaoNaoPermitidaExcecao($"Sala '{sala.Nome}' ja existe neste cinema.");
            }

            sala.Id = salas.Count > 0 ? salas.Max(s => s.Id) + 1 : 1;
            // Gera assentos especiais com base nas quantidades da sala.
            sala.Assentos = GeradorDeLugares.GerarAssentos(
                sala.Capacidade,
                sala,
                sala.QuantidadeAssentosCasal,
                sala.QuantidadeAssentosPCD);
            salas.Add(sala);

            if (!sala.Cinema.Salas.Contains(sala))
            {
                sala.Cinema.Salas.Add(sala);
            }
        }

        public Sala ObterSala(int id)
        {
            var sala = salas.FirstOrDefault(s => s.Id == id);
            if (sala == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Sala com ID {id} não encontrada.");
            }
            return sala;
        }

        public List<Sala> ListarSalas()
        {
            return new List<Sala>(salas);
        }

        public List<Sala> ListarSalasPorCinema(int cinemaId)
        {
            return salas.Where(s => s.Cinema != null && s.Cinema.Id == cinemaId).ToList();
        }

        public void AtualizarSala(int id, string? nome = null, int? capacidade = null,
            int? quantidadeAssentosCasal = null, int? quantidadeAssentosPCD = null)
        {
            var sala = ObterSala(id);

            if (!string.IsNullOrWhiteSpace(nome))
            {
                if (salas.Any(s =>
                    s.Id != id &&
                    s.Cinema != null &&
                    sala.Cinema != null &&
                    s.Cinema.Id == sala.Cinema.Id &&
                    s.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new OperacaoNaoPermitidaExcecao($"Ja existe uma sala com o nome '{nome}' neste cinema.");
                }
                sala.Nome = nome;
            }

            if (quantidadeAssentosCasal.HasValue)
            {
                if (quantidadeAssentosCasal.Value < 0)
                {
                    throw new DadosInvalidosExcecao("Quantidade de assentos casal invalida.");
                }
                sala.QuantidadeAssentosCasal = quantidadeAssentosCasal.Value;
            }

            if (quantidadeAssentosPCD.HasValue)
            {
                if (quantidadeAssentosPCD.Value < 0)
                {
                    throw new DadosInvalidosExcecao("Quantidade de assentos PCD invalida.");
                }
                sala.QuantidadeAssentosPCD = quantidadeAssentosPCD.Value;
            }

            if (capacidade.HasValue)
            {
                if (capacidade.Value <= 0)
                {
                    throw new DadosInvalidosExcecao("Capacidade deve ser maior que zero.");
                }
                bool capacidadeMudou = sala.Capacidade != capacidade.Value;
                sala.Capacidade = capacidade.Value;

                if (capacidadeMudou)
                {
                    sala.Assentos = GeradorDeLugares.GerarAssentos(
                        sala.Capacidade,
                        sala,
                        sala.QuantidadeAssentosCasal,
                        sala.QuantidadeAssentosPCD);
                }
            }

            int lugaresEspeciais = sala.QuantidadeAssentosPCD + (sala.QuantidadeAssentosCasal * 2);
            if (lugaresEspeciais > sala.Capacidade)
            {
                throw new DadosInvalidosExcecao("Quantidade de assentos especiais excede a capacidade.");
            }

            ResetarAssentosDisponiveis(sala);
        }

        // Geração manual de assentos
        public void GerarAssentosParaSala(int id)
        {
            var sala = ObterSala(id);
            
            if (sala.Capacidade <= 0)
            {
                throw new DadosInvalidosExcecao("Capacidade da sala deve ser maior que zero.");
            }

            sala.Assentos = GeradorDeLugares.GerarAssentos(
                sala.Capacidade,
                sala,
                sala.QuantidadeAssentosCasal,
                sala.QuantidadeAssentosPCD);
        }

        public void DeletarSala(int id)
        {
            var sala = ObterSala(id);
            salas.Remove(sala);

            if (sala.Cinema != null)
            {
                sala.Cinema.Salas.Remove(sala);
            }
        }

        // Helper - define todos os assentos como disponíveis
        private static void ResetarAssentosDisponiveis(Sala sala)
        {
            foreach (var assento in sala.Assentos)
            {
                assento.Liberar();
            }
        }
    }
}




