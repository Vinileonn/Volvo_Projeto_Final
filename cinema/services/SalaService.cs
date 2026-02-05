using cinema.models;
using cinema.utils;
using cinema.exceptions;

namespace cinema.services
{
    public class SalaService
    {
        private readonly List<Sala> salas;

        public SalaService()
        {
            salas = new List<Sala>();
        }

        // CREATE - cadastrar nova sala e gerar assentos
        public void CriarSala(Sala sala)
        {
            if (sala == null)
            {
                throw new DadosInvalidosException("Sala não pode ser nula.");
            }

            if (string.IsNullOrWhiteSpace(sala.Nome))
            {
                throw new DadosInvalidosException("Nome da sala é obrigatório.");
            }

            if (sala.Capacidade <= 0)
            {
                throw new DadosInvalidosException("Capacidade deve ser maior que zero.");
            }

            if (salas.Any(s => s.Nome.Equals(sala.Nome, StringComparison.OrdinalIgnoreCase)))
            {
                throw new OperacaoNaoPermitidaException($"Sala '{sala.Nome}' já existe.");
            }

            sala.Id = salas.Count > 0 ? salas.Max(s => s.Id) + 1 : 1;
            sala.Assentos = GeradorDeLugares.GerarAssentos(sala.Capacidade, sala);
            salas.Add(sala);
        }

        // READ - obter sala por id
        public Sala ObterSala(int id)
        {
            var sala = salas.FirstOrDefault(s => s.Id == id);
            if (sala == null)
            {
                throw new RecursoNaoEncontradoException($"Sala com ID {id} não encontrada.");
            }
            return sala;
        }

        // READ - listar todas as salas
        public List<Sala> ListarSalas()
        {
            return new List<Sala>(salas);
        }

        // UPDATE - atualizar dados da sala (regenera assentos se capacidade mudar)
        public void AtualizarSala(int id, string? nome = null, int? capacidade = null)
        {
            var sala = ObterSala(id);

            if (!string.IsNullOrWhiteSpace(nome))
            {
                if (salas.Any(s => s.Id != id && s.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new OperacaoNaoPermitidaException($"Já existe uma sala com o nome '{nome}'.");
                }
                sala.Nome = nome;
            }

            if (capacidade.HasValue)
            {
                if (capacidade.Value <= 0)
                {
                    throw new DadosInvalidosException("Capacidade deve ser maior que zero.");
                }
                bool capacidadeMudou = sala.Capacidade != capacidade.Value;
                sala.Capacidade = capacidade.Value;

                if (capacidadeMudou)
                {
                    sala.Assentos = GeradorDeLugares.GerarAssentos(sala.Capacidade, sala);
                }
            }

            ResetarAssentosDisponiveis(sala);
        }

        // Geração manual de assentos
        public void GerarAssentosParaSala(int id)
        {
            var sala = ObterSala(id);
            
            if (sala.Capacidade <= 0)
            {
                throw new DadosInvalidosException("Capacidade da sala deve ser maior que zero.");
            }

            sala.Assentos = GeradorDeLugares.GerarAssentos(sala.Capacidade, sala);
        }

        // DELETE - remover sala
        public void DeletarSala(int id)
        {
            var sala = ObterSala(id);
            salas.Remove(sala);
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