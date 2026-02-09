using cinecore.dados;
using cinecore.modelos;
using Microsoft.EntityFrameworkCore;

namespace cinecore.servicos
{
    /// <summary>
    /// Serviço de lógica de negócio para Cinemas
    /// </summary>
    public class CinemaServico
    {
        private readonly CineFlowContext _context;

        public CinemaServico(CineFlowContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo cinema com validações
        /// </summary>
        public Cinema CriarCinema(Cinema cinema)
        {
            if (cinema == null)
                throw new ArgumentNullException(nameof(cinema), "Cinema não pode ser nulo.");

            ValidarCamposObrigatorios(cinema);
            ValidarDuplicidade(cinema);

            cinema.DataCriacao = DateTime.Now;
            
            // Inicializa as listas se ainda não foram inicializadas
            if (cinema.Salas == null)
                cinema.Salas = new List<Sala>();
            
            if (cinema.Funcionarios == null)
                cinema.Funcionarios = new List<Funcionario>();
            
            _context.Cinemas.Add(cinema);
            _context.SaveChanges();

            return cinema;
        }

        /// <summary>
        /// Obtém um cinema pelo ID
        /// </summary>
        public Cinema ObterCinema(int id)
        {
            var cinema = _context.Cinemas
                .Include(c => c.Salas)
                .Include(c => c.Funcionarios)
                .FirstOrDefault(c => c.Id == id);
            if (cinema == null)
                throw new KeyNotFoundException($"Cinema com ID {id} não encontrado.");

            return cinema;
        }

        /// <summary>
        /// Lista todos os cinemas
        /// </summary>
        public List<Cinema> ListarCinemas()
        {
            return _context.Cinemas
                .Include(c => c.Salas)
                .Include(c => c.Funcionarios)
                .ToList();
        }

        /// <summary>
        /// Busca cinemas por nome
        /// </summary>
        public List<Cinema> BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return new List<Cinema>();

            return _context.Cinemas
                .Include(c => c.Salas)
                .Include(c => c.Funcionarios)
                .Where(c => c.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Atualiza um cinema existente
        /// </summary>
        public Cinema AtualizarCinema(int id, Cinema cinemaAtualizado)
        {
            var cinema = ObterCinema(id);

            // Valida nome duplicado se estiver sendo alterado
            if (!string.IsNullOrWhiteSpace(cinemaAtualizado.Nome) &&
                !cinema.Nome.Equals(cinemaAtualizado.Nome, StringComparison.OrdinalIgnoreCase))
            {
                ValidarDuplicidadeNome(cinemaAtualizado.Nome, id);
            }

            // Atualiza campos se fornecidos
            if (!string.IsNullOrWhiteSpace(cinemaAtualizado.Nome))
                cinema.Nome = cinemaAtualizado.Nome;

            if (!string.IsNullOrWhiteSpace(cinemaAtualizado.Endereco))
                cinema.Endereco = cinemaAtualizado.Endereco;

            cinema.DataAtualizacao = DateTime.Now;

            _context.SaveChanges();

            return cinema;
        }

        /// <summary>
        /// Deleta um cinema
        /// </summary>
        public void DeletarCinema(int id)
        {
            var cinema = ObterCinema(id);
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
        }

        /// <summary>
        /// Obtém as salas de um cinema
        /// </summary>
        public List<Sala> ObterSalasDoCinema(int id)
        {
            var cinema = ObterCinema(id);
            return new List<Sala>(cinema.Salas);
        }

        /// <summary>
        /// Obtém os funcionários de um cinema
        /// </summary>
        public List<Funcionario> ObterFuncionariosDoCinema(int id)
        {
            var cinema = ObterCinema(id);
            return new List<Funcionario>(cinema.Funcionarios);
        }

        // ===== MÉTODOS PRIVADOS DE VALIDAÇÃO =====

        private void ValidarCamposObrigatorios(Cinema cinema)
        {
            var camposVazios = new List<string>();

            if (string.IsNullOrWhiteSpace(cinema.Nome))
                camposVazios.Add("nome");

            if (string.IsNullOrWhiteSpace(cinema.Endereco))
                camposVazios.Add("endereço");

            if (camposVazios.Count > 0)
            {
                throw new ArgumentException(
                    $"Campos obrigatórios faltando: {string.Join(", ", camposVazios)}.");
            }
        }

        private void ValidarDuplicidade(Cinema cinema)
        {
            ValidarDuplicidadeNome(cinema.Nome);
        }

        private void ValidarDuplicidadeNome(string nome, int? idParaIgnorar = null)
        {
            var jaExiste = _context.Cinemas.Any(c =>
                (!idParaIgnorar.HasValue || c.Id != idParaIgnorar.Value) &&
                c.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

            if (jaExiste)
            {
                throw new InvalidOperationException(
                    $"Cinema '{nome}' já existe.");
            }
        }
    }
}
