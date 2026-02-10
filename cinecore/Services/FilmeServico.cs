using cinecore.Data;
using cinecore.Models;
using cinecore.Enums;
using Microsoft.EntityFrameworkCore;

namespace cinecore.Services
{
    /// <summary>
    /// Serviço de lógica de negócio para Filmes
    /// </summary>
    public class FilmeServico
    {
        private readonly CineFlowContext _context;

        public FilmeServico(CineFlowContext context)
        {
            _context = context;
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

            filme.DataCriacao = DateTime.Now;
            
            // Inicializa a lista de sessões se ainda não foi inicializada
            if (filme.Sessoes == null)
                filme.Sessoes = new List<Sessao>();
            
            _context.Filmes.Add(filme);
            _context.SaveChanges();

            return filme;
        }

        /// <summary>
        /// Obtém um filme pelo ID
        /// </summary>
        public Filme ObterFilme(int id)
        {
            var filme = _context.Filmes
                .Include(f => f.Sessoes)
                .FirstOrDefault(f => f.Id == id);
            if (filme == null)
                throw new KeyNotFoundException($"Filme com ID {id} não encontrado.");

            return filme;
        }

        /// <summary>
        /// Lista todos os filmes
        /// </summary>
        public List<Filme> ListarFilmes()
        {
            return _context.Filmes
                .Include(f => f.Sessoes)
                .ToList();
        }

        /// <summary>
        /// Busca filmes por título
        /// </summary>
        public List<Filme> BuscarPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return new List<Filme>();

            var tituloLower = titulo.ToLower();
            return _context.Filmes
                .Include(f => f.Sessoes)
                .Where(f => f.Titulo.ToLower().Contains(tituloLower))
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
                !filme.Titulo.ToLower().Equals(filmeAtualizado.Titulo.ToLower()))
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

            _context.SaveChanges();

            return filme;
        }

        /// <summary>
        /// Deleta um filme
        /// </summary>
        public void DeletarFilme(int id)
        {
            var filme = ObterFilme(id);
            _context.Filmes.Remove(filme);
            _context.SaveChanges();
        }

        /// <summary>
        /// Obtém as sessões de um filme
        /// </summary>
        public List<Sessao> ObterSessoesDoFilme(int id)
        {
            var filme = ObterFilme(id);
            return new List<Sessao>(filme.Sessoes);
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
            var tituloLower = titulo.ToLower();
            var jaExiste = _context.Filmes.Any(f =>
                (!idParaIgnorar.HasValue || f.Id != idParaIgnorar.Value) &&
                f.Titulo.ToLower().Equals(tituloLower) &&
                f.AnoLancamento.Year == anoLancamento.Year);

            if (jaExiste)
            {
                throw new InvalidOperationException(
                    $"Filme '{titulo}' ({anoLancamento.Year}) já existe.");
            }
        }
    }
}
