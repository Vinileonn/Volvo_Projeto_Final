using cinecore.modelos;

namespace cinecore.servicos
{
    /// <summary>
    /// Serviço de lógica de negócio para Cinemas
    /// </summary>
    public class CinemaServico
    {
        private readonly List<Cinema> _cinemas;

        public CinemaServico()
        {
            _cinemas = new List<Cinema>();
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

            cinema.Id = _cinemas.Count > 0 ? _cinemas.Max(c => c.Id) + 1 : 1;
            cinema.DataCriacao = DateTime.Now;
            
            // Inicializa as listas se ainda não foram inicializadas
            if (cinema.Salas == null)
                cinema.Salas = new List<Sala>();
            
            if (cinema.Funcionarios == null)
                cinema.Funcionarios = new List<Funcionario>();
            
            _cinemas.Add(cinema);

            return cinema;
        }

        /// <summary>
        /// Obtém um cinema pelo ID
        /// </summary>
        public Cinema ObterCinema(int id)
        {
            var cinema = _cinemas.FirstOrDefault(c => c.Id == id);
            if (cinema == null)
                throw new KeyNotFoundException($"Cinema com ID {id} não encontrado.");

            return cinema;
        }

        /// <summary>
        /// Lista todos os cinemas
        /// </summary>
        public List<Cinema> ListarCinemas()
        {
            return new List<Cinema>(_cinemas);
        }

        /// <summary>
        /// Busca cinemas por nome
        /// </summary>
        public List<Cinema> BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return new List<Cinema>();

            return _cinemas
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

            return cinema;
        }

        /// <summary>
        /// Deleta um cinema
        /// </summary>
        public void DeletarCinema(int id)
        {
            var cinema = ObterCinema(id);
            _cinemas.Remove(cinema);
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
            var jaExiste = _cinemas.Any(c =>
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
