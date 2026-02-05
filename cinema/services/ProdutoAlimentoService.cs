using cinema.models;
using cinema.exceptions;
namespace cinema.services
{
    public class ProdutoAlimentoService
    {
        private readonly List<ProdutoAlimento> produtos;

        public ProdutoAlimentoService()
        {
            produtos = new List<ProdutoAlimento>();
        }

        // CREATE - cadastrar novo produto
        public void CriarProduto(ProdutoAlimento produto)
        {
            if (produto == null)
            {
                throw new DadosInvalidosException("Produto não pode ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(produto.Nome) || produto.Preco < 0 || 
                produto.EstoqueAtual < 0 || produto.EstoqueMinimo < 0)
            {
                throw new DadosInvalidosException("Dados do produto inválidos.");
            }

            if (produtos.Any(p => p.Nome.Equals(produto.Nome, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DadosInvalidosException("Produto com o mesmo nome já existe.");
            }

            produto.Id = produtos.Count > 0 ? produtos.Max(p => p.Id) + 1 : 1;
            produtos.Add(produto);
        }

        // READ - obter produto por id
        public ProdutoAlimento? ObterProduto(int id)
        {
            return produtos.FirstOrDefault(p => p.Id == id);
        }

        // READ - listar todos os produtos
        public List<ProdutoAlimento> ListarProdutos()
        {
            return new List<ProdutoAlimento>(produtos);
        }

        // READ - buscar por nome
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

        // READ - listar produtos com estoque baixo
        public List<ProdutoAlimento> ListarProdutosEstoqueBaixo()
        {
            return produtos.Where(p => p.EstoqueAtual <= p.EstoqueMinimo).ToList();
        }

        // UPDATE - atualizar dados do produto
        public void AtualizarProduto(int id, string? nome = null, string? descricao = null,
                                     float? preco = null, int? estoqueMinimo = null)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoException($"Produto com ID {id} não encontrado.");
            }

            if (!string.IsNullOrWhiteSpace(nome))
            {
                if (produtos.Any(p => p.Id != id && 
                    p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new DadosInvalidosException("Produto com o mesmo nome já existe.");
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
                    throw new DadosInvalidosException("Preço não pode ser negativo.");
                }
                produto.Preco = preco.Value;
            }

            if (estoqueMinimo.HasValue)
            {
                if (estoqueMinimo.Value < 0)
                {
                    throw new DadosInvalidosException("Estoque mínimo não pode ser negativo.");
                }
                produto.EstoqueMinimo = estoqueMinimo.Value;
            }
        }

        // DELETE - remover produto
        public void DeletarProduto(int id)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoException($"Produto com ID {id} não encontrado.");
            }

            produtos.Remove(produto);
        }

        // ESTOQUE - adicionar quantidade ao estoque
        public void AdicionarEstoque(int id, int quantidade)
        {
            if (quantidade <= 0)
            {
                throw new DadosInvalidosException("Quantidade deve ser maior que zero.");
            }

            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoException($"Produto com ID {id} não encontrado.");
            }

            produto.EstoqueAtual += quantidade;
        }

        // ESTOQUE - reduzir quantidade do estoque (para vendas)
        public void ReduzirEstoque(int id, int quantidade)
        {
            if (quantidade <= 0)
            {
                throw new DadosInvalidosException("Quantidade deve ser maior que zero.");
            }

            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                throw new RecursoNaoEncontradoException($"Produto com ID {id} não encontrado.");
            }

            if (produto.EstoqueAtual < quantidade)
            {
                throw new DadosInvalidosException("Quantidade insuficiente em estoque.");
            }

            produto.EstoqueAtual -= quantidade;
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
