using cinecore.modelos;
using cinecore.enums;

namespace cinecore.servicos
{
    /// <summary>
    /// Serviço de lógica de negócio para Filmes
    /// </summary>
    public class FilmeServico
    {
        private readonly List<Filme> _filmes;

        public FilmeServico()
        {
            _filmes = new List<Filme>();
        }

        /// <summary>
        /// Cria um novo filme com validações
        /// </summary>
        public Filme CriarFilme(Filme filme)
        {
            if (filme == null)
                throw new ArgumentNullException(nameof(filme), "Filme não pode ser nulo.");

            ValidarCamposObrigatorios(filme);
            ValidarDuracao(filme.Duracao);
            ValidarDuplicidade(filme);

            filme.Id = _filmes.Count > 0 ? _filmes.Max(f => f.Id) + 1 : 1;
            filme.DataCriacao = DateTime.Now;
            _filmes.Add(filme);

            return filme;
        }

        /// <summary>
        /// Obtém um filme pelo ID
        /// </summary>
        public Filme ObterFilme(int id)
        {
            var filme = _filmes.FirstOrDefault(f => f.Id == id);
            if (filme == null)
                throw new KeyNotFoundException($"Filme com ID {id} não encontrado.");

            return filme;
        }

        /// <summary>
        /// Lista todos os filmes
        /// </summary>
        public List<Filme> ListarFilmes()
        {
            return new List<Filme>(_filmes);
        }

        /// <summary>
        /// Busca filmes por título
        /// </summary>
        public List<Filme> BuscarPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return new List<Filme>();

            return _filmes
                .Where(f => f.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Atualiza um filme existente
        /// </summary>
        public Filme AtualizarFilme(int id, Filme filmeAtualizado)
        {
            var filme = ObterFilme(id);

            // Valida título duplicado se estiver sendo alterado
            if (!string.IsNullOrWhiteSpace(filmeAtualizado.Titulo) &&
                !filme.Titulo.Equals(filmeAtualizado.Titulo, StringComparison.OrdinalIgnoreCase))
            {
                var anoParaValidar = filmeAtualizado.AnoLancamento != default 
                    ? filmeAtualizado.AnoLancamento 
                    : filme.AnoLancamento;
                
                ValidarDuplicidade(filmeAtualizado.Titulo, anoParaValidar, id);
            }

            // Valida e atualiza duração
            if (filmeAtualizado.Duracao > 0 && filmeAtualizado.Duracao != filme.Duracao)
            {
                ValidarDuracao(filmeAtualizado.Duracao);
                filme.Duracao = filmeAtualizado.Duracao;
            }

            // Valida e atualiza ano de lançamento
            if (filmeAtualizado.AnoLancamento != default && filmeAtualizado.AnoLancamento != filme.AnoLancamento)
            {
                filme.AnoLancamento = filmeAtualizado.AnoLancamento;
            }

            // Atualiza campos opcionais
            if (!string.IsNullOrWhiteSpace(filmeAtualizado.Titulo))
                filme.Titulo = filmeAtualizado.Titulo;

            if (!string.IsNullOrWhiteSpace(filmeAtualizado.Genero))
                filme.Genero = filmeAtualizado.Genero;

            filme.Eh3D = filmeAtualizado.Eh3D;
            filme.Classificacao = filmeAtualizado.Classificacao;
            filme.DataAtualizacao = DateTime.Now;

            return filme;
        }

        /// <summary>
        /// Deleta um filme
        /// </summary>
        public void DeletarFilme(int id)
        {
            var filme = ObterFilme(id);
            _filmes.Remove(filme);
        }

        // ===== MÉTODOS PRIVADOS DE VALIDAÇÃO =====

        private void ValidarCamposObrigatorios(Filme filme)
        {
            var camposVazios = new List<string>();

            if (string.IsNullOrWhiteSpace(filme.Titulo))
                camposVazios.Add("título");

            if (string.IsNullOrWhiteSpace(filme.Genero))
                camposVazios.Add("gênero");

            if (filme.AnoLancamento == default)
                camposVazios.Add("ano de lançamento");

            if (camposVazios.Count > 0)
            {
                throw new ArgumentException(
                    $"Campos obrigatórios faltando: {string.Join(", ", camposVazios)}.");
            }
        }

        private void ValidarDuracao(int duracao)
        {
            if (duracao <= 0)
                throw new ArgumentException("Duração deve ser maior que zero.");
        }

        private void ValidarDuplicidade(Filme filme)
        {
            ValidarDuplicidade(filme.Titulo, filme.AnoLancamento);
        }

        private void ValidarDuplicidade(string titulo, DateTime anoLancamento, int? idParaIgnorar = null)
        {
            var jaExiste = _filmes.Any(f =>
                (!idParaIgnorar.HasValue || f.Id != idParaIgnorar.Value) &&
                f.Titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase) &&
                f.AnoLancamento.Year == anoLancamento.Year);

            if (jaExiste)
            {
                throw new InvalidOperationException(
                    $"Filme '{titulo}' ({anoLancamento.Year}) já existe.");
            }
        }
    }
}
