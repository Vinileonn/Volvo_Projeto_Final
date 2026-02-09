using cinecore.dados;
using cinecore.modelos;
using cinecore.excecoes;
using cinecore.enums;

namespace cinecore.servicos
{
    public class ProdutoAlimentoServico
    {
        private readonly CineFlowContext _context;
        private readonly List<string> alertasEstoque;

        public ProdutoAlimentoServico(CineFlowContext context)
        {
            _context = context;
            alertasEstoque = new List<string>();
        }

        public void CriarProduto(ProdutoAlimento produto)
        {
            if (produto == null)
            {
                throw new DadosInvalidosExcecao("Produto não pode ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(produto.Nome) || produto.Preco < 0 || 
                produto.EstoqueAtual < 0 || produto.EstoqueMinimo < 0)
            {
                throw new DadosInvalidosExcecao("Dados do produto inválidos.");
            }

            if (_context.ProdutosAlimento.Any(p => p.Nome.ToLower().Equals(produto.Nome.ToLower())))
            {
                throw new DadosInvalidosExcecao("Produto com o mesmo nome já existe.");
            }

            if (produto.Categoria == CategoriaProduto.Cortesia)
            {
                produto.EhCortesia = true;
            }

            if (produto.EhCortesia && produto.Preco > 0)
            {
                throw new DadosInvalidosExcecao("Cortesia deve ter preço zero.");
            }

            if ((produto.EhTematico || produto.Categoria == CategoriaProduto.Tematico) && string.IsNullOrWhiteSpace(produto.TemaFilme))
            {
                throw new DadosInvalidosExcecao("Produto temático precisa de tema do filme.");
            }

            _context.ProdutosAlimento.Add(produto);
            _context.SaveChanges();
        }

        public ProdutoAlimento? ObterProduto(int id)
        {
            return _context.ProdutosAlimento.FirstOrDefault(p => p.Id == id);
        }

        public ProdutoAlimento? ObterCortesiaPreEstreiaDisponivel()
        {
            return _context.ProdutosAlimento
                .FirstOrDefault(p => p.EhCortesia && p.ExclusivoPreEstreia && p.EstoqueAtual > 0);
        }

        public List<ProdutoAlimento> ListarProdutos()
        {
            return _context.ProdutosAlimento.ToList();
        }

        public List<ProdutoAlimento> BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return new List<ProdutoAlimento>();
            }

            var nomeLower = nome.ToLower();
            return _context.ProdutosAlimento
                .Where(p => p.Nome.ToLower().Contains(nomeLower))
                .ToList();
        }

        public List<ProdutoAlimento> ListarProdutosEstoqueBaixo()
        {
            return _context.ProdutosAlimento
                .Where(p => p.EstoqueAtual <= p.EstoqueMinimo)
                .ToList();
        }

        public List<string> ListarAlertasEstoque()
        {
            return new List<string>(alertasEstoque);
        }

        public void AtualizarProduto(int id, string? nome = null, string? descricao = null,
                                     float? preco = null, int? estoqueMinimo = null, bool? ehTematico = null,
                                     string? temaFilme = null, bool? ehCortesia = null, bool? exclusivoPreEstreia = null,
                                     CategoriaProduto? categoria = null)
        {
            var produto = _context.ProdutosAlimento.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Produto com ID {id} não encontrado.");
            }

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var nomeLower = nome.ToLower();
                if (_context.ProdutosAlimento.Any(p => p.Id != id && 
                    p.Nome.ToLower().Equals(nomeLower)))
                {
                    throw new DadosInvalidosExcecao("Produto com o mesmo nome já existe.");
                }
                produto.Nome = nome;
            }

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                produto.Descricao = descricao;
            }

            if (preco.HasValue)
            {
                if (preco.Value < 0)
                {
                    throw new DadosInvalidosExcecao("Preço não pode ser negativo.");
                }
                produto.Preco = preco.Value;
            }

            if (estoqueMinimo.HasValue)
            {
                if (estoqueMinimo.Value < 0)
                {
                    throw new DadosInvalidosExcecao("Estoque mínimo não pode ser negativo.");
                }
                produto.EstoqueMinimo = estoqueMinimo.Value;
            }

            if (categoria.HasValue)
            {
                produto.Categoria = categoria.Value;
                if (produto.Categoria == CategoriaProduto.Cortesia)
                {
                    produto.EhCortesia = true;
                }
            }

            if (ehTematico.HasValue)
            {
                produto.EhTematico = ehTematico.Value;
            }

            if (temaFilme != null)
            {
                produto.TemaFilme = string.IsNullOrWhiteSpace(temaFilme) ? null : temaFilme;
            }

            if (ehCortesia.HasValue)
            {
                produto.EhCortesia = ehCortesia.Value;
            }

            if (exclusivoPreEstreia.HasValue)
            {
                produto.ExclusivoPreEstreia = exclusivoPreEstreia.Value;
            }

            if (produto.EhCortesia && produto.Preco > 0)
            {
                throw new DadosInvalidosExcecao("Cortesia deve ter preço zero.");
            }

            if ((produto.EhTematico || produto.Categoria == CategoriaProduto.Tematico) && string.IsNullOrWhiteSpace(produto.TemaFilme))
            {
                throw new DadosInvalidosExcecao("Produto temático precisa de tema do filme.");
            }

            _context.SaveChanges();
        }

        public void DeletarProduto(int id)
        {
            var produto = _context.ProdutosAlimento.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Produto com ID {id} não encontrado.");
            }

            _context.ProdutosAlimento.Remove(produto);
            _context.SaveChanges();
        }

        // ESTOQUE - adicionar quantidade ao estoque
        public void AdicionarEstoque(int id, int quantidade)
        {
            if (quantidade <= 0)
            {
                throw new DadosInvalidosExcecao("Quantidade deve ser maior que zero.");
            }

            var produto = _context.ProdutosAlimento.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Produto com ID {id} não encontrado.");
            }

            produto.EstoqueAtual += quantidade;
            _context.SaveChanges();
        }

        // ESTOQUE - reduzir quantidade do estoque (para vendas)
        public void ReduzirEstoque(int id, int quantidade)
        {
            if (quantidade <= 0)
            {
                throw new DadosInvalidosExcecao("Quantidade deve ser maior que zero.");
            }

            var produto = _context.ProdutosAlimento.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Produto com ID {id} não encontrado.");
            }

            if (produto.EstoqueAtual < quantidade)
            {
                throw new DadosInvalidosExcecao("Quantidade insuficiente em estoque.");
            }

            produto.EstoqueAtual -= quantidade;

            if (produto.EstoqueAtual <= produto.EstoqueMinimo)
            {
                alertasEstoque.Add($"Estoque baixo: {produto.Nome} ({produto.EstoqueAtual})");
            }

            _context.SaveChanges();
        }

        // ESTOQUE - verificar disponibilidade
        public bool VerificarDisponibilidade(int id, int quantidade)
        {
            var produto = _context.ProdutosAlimento.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                return false;
            }

            return produto.EstoqueAtual >= quantidade;
        }
    }
}
