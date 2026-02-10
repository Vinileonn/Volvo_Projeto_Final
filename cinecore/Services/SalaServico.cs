using cinecore.Data;
using cinecore.Models;
using cinecore.Utilities;
using cinecore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Services
{
    public class SalaServico
    {
        private readonly CineFlowContext _context;

        public SalaServico(CineFlowContext context)
        {
            _context = context;
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

            if (sala.QuantidadeAssentosCasal < 0 || sala.QuantidadeAssentosPCD < 0)
            {
                throw new DadosInvalidosExcecao("Quantidade de assentos especiais invalida.");
            }

            int lugaresEspeciais = sala.QuantidadeAssentosPCD + (sala.QuantidadeAssentosCasal * 2);
            if (lugaresEspeciais > sala.Capacidade)
            {
                throw new DadosInvalidosExcecao("Quantidade de assentos especiais excede a capacidade.");
            }

            if (sala.Cinema != null && _context.Salas.Any(s =>
                s.Cinema != null &&
                s.Cinema.Id == sala.Cinema.Id &&
                s.Nome.ToLower().Equals(sala.Nome.ToLower())))
            {
                throw new OperacaoNaoPermitidaExcecao($"Sala '{sala.Nome}' ja existe neste cinema.");
            }
            
            // Gera assentos especiais com base nas quantidades da sala
            sala.Assentos = GeradorDeLugares.GerarAssentos(
                sala.Capacidade,
                sala,
                sala.QuantidadeAssentosCasal,
                sala.QuantidadeAssentosPCD);
            
            _context.Salas.Add(sala);
            _context.SaveChanges();
        }

        public Sala ObterSala(int id)
        {
            var sala = _context.Salas
                .Include(s => s.Cinema)
                    .ThenInclude(c => c!.Funcionarios)
                .Include(s => s.Assentos)
                .FirstOrDefault(s => s.Id == id);
            if (sala == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Sala com ID {id} não encontrada.");
            }
            return sala;
        }

        public List<Sala> ListarSalas()
        {
            return _context.Salas
                .Include(s => s.Cinema)
                .Include(s => s.Assentos)
                .ToList();
        }

        public List<Sala> ListarSalasPorCinema(int cinemaId)
        {
            return _context.Salas
                .Include(s => s.Cinema)
                .Include(s => s.Assentos)
                .Where(s => s.Cinema != null && s.Cinema.Id == cinemaId)
                .ToList();
        }

        public void AtualizarSala(int id, string? nome = null, int? capacidade = null,
            int? quantidadeAssentosCasal = null, int? quantidadeAssentosPCD = null)
        {
            var sala = ObterSala(id);

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var nomeLower = nome.ToLower();
                if (_context.Salas.Any(s =>
                    s.Id != id &&
                    s.Cinema != null &&
                    sala.Cinema != null &&
                    s.Cinema.Id == sala.Cinema.Id &&
                    s.Nome.ToLower().Equals(nomeLower)))
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
                    if (sala.Assentos.Count > 0)
                    {
                        _context.Assentos.RemoveRange(sala.Assentos);
                    }

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
            sala.DataAtualizacao = DateTime.Now;

            _context.SaveChanges();
        }

        public void DeletarSala(int id)
        {
            var sala = ObterSala(id);
            _context.Salas.Remove(sala);
            _context.SaveChanges();
        }

        /// <summary>
        /// Helper - define todos os assentos de uma sala como disponíveis
        /// </summary>
        private static void ResetarAssentosDisponiveis(Sala sala)
        {
            foreach (var assento in sala.Assentos)
            {
                assento.Liberar();
            }
        }

        /// <summary>
        /// Retorna a visualização da sala com assentos organizados por fila
        /// </summary>
        public Dictionary<string, List<string>> VisualizarSala(int id)
        {
            var sala = ObterSala(id);
            var visualizacao = new Dictionary<string, List<string>>();

            // Agrupa assentos por fila e ordena
            var assentosPorFila = sala.Assentos
                .GroupBy(a => a.Fila)
                .OrderBy(g => g.Key);

            foreach (var grupo in assentosPorFila)
            {
                var fila = grupo.Key.ToString();
                var assentosNaFila = grupo
                    .OrderBy(a => a.Numero)
                    .Select(a => a.Disponivel ? $"{a.Fila}{a.Numero}" : "X")
                    .ToList();

                visualizacao[fila] = assentosNaFila;
            }

            return visualizacao;
        }
    }
}
