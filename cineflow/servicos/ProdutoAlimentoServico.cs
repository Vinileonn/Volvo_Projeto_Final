using cineflow.modelos;
using cineflow.excecoes;
using cineflow.enumeracoes;
namespace cineflow.servicos
{
    public class ProdutoAlimentoServico
    {
        private readonly List<ProdutoAlimento> produtos;
        private readonly List<string> alertasEstoque;

        public ProdutoAlimentoServico()
        {
            produtos = new List<ProdutoAlimento>();
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

            if (produtos.Any(p => p.Nome.Equals(produto.Nome, StringComparison.OrdinalIgnoreCase)))
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

            produto.Id = produtos.Count > 0 ? produtos.Max(p => p.Id) + 1 : 1;
            produtos.Add(produto);
        }

        public ProdutoAlimento? ObterProduto(int id)
        {
            return produtos.FirstOrDefault(p => p.Id == id);
        }

        public ProdutoAlimento? ObterCortesiaPreEstreiaDisponivel()
        {
            return produtos.FirstOrDefault(p => p.EhCortesia && p.ExclusivoPreEstreia && p.EstoqueAtual > 0);
        }

        public List<ProdutoAlimento> ListarProdutos()
        {
            return new List<ProdutoAlimento>(produtos);
        }

        public List<ProdutoAlimento> BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return new List<ProdutoAlimento>();
            }

            return produtos
                .Where(p => p.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<ProdutoAlimento> ListarProdutosEstoqueBaixo()
        {
            return produtos.Where(p => p.EstoqueAtual <= p.EstoqueMinimo).ToList();
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
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Produto com ID {id} não encontrado.");
            }

            if (!string.IsNullOrWhiteSpace(nome))
            {
                if (produtos.Any(p => p.Id != id && 
                    p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
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
        }

        public void DeletarProduto(int id)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Produto com ID {id} não encontrado.");
            }

            produtos.Remove(produto);
        }

        // ESTOQUE - adicionar quantidade ao estoque
        public void AdicionarEstoque(int id, int quantidade)
        {
            if (quantidade <= 0)
            {
                throw new DadosInvalidosExcecao("Quantidade deve ser maior que zero.");
            }

            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Produto com ID {id} não encontrado.");
            }

            produto.EstoqueAtual += quantidade;
        }

        // ESTOQUE - reduzir quantidade do estoque (para vendas)
        public void ReduzirEstoque(int id, int quantidade)
        {
            if (quantidade <= 0)
            {
                throw new DadosInvalidosExcecao("Quantidade deve ser maior que zero.");
            }

            var produto = produtos.FirstOrDefault(p => p.Id == id);
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
        }

        // ESTOQUE - verificar disponibilidade
        public bool VerificarDisponibilidade(int id, int quantidade)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                return false;
            }

            return produto.EstoqueAtual >= quantidade;
        }
    }
}





