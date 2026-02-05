using cinema.models;
using cinema.services;
using cinema.exceptions;

namespace cinema.controllers
{
    public class ProdutoController
    {
        private readonly ProdutoAlimentoService produtoService;

        public ProdutoController(ProdutoAlimentoService produtoService)
        {
            this.produtoService = produtoService;
        }

        // CREATE - cadastrar novo produto
        public (bool sucesso, string mensagem) CriarProduto(ProdutoAlimento produto)
        {
            try
            {
                produtoService.CriarProduto(produto);
                return (true, $"Produto '{produto.Nome}' cadastrado com sucesso.");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao cadastrar produto.");
            }
        }

        // READ - obter produto por id
        public (ProdutoAlimento? produto, string mensagem) ObterProduto(int id)
        {
            try
            {
                var produto = produtoService.ObterProduto(id);
                if (produto == null)
                {
                    return (null, $"Produto com ID {id} não encontrado.");
                }
                return (produto, "Produto obtido com sucesso.");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter produto.");
            }
        }

        // READ - listar todos os produtos
        public (List<ProdutoAlimento> produtos, string mensagem) ListarProdutos()
        {
            try
            {
                var produtos = produtoService.ListarProdutos();
                if (produtos.Count == 0)
                {
                    return (produtos, "Nenhum produto cadastrado.");
                }
                return (produtos, $"{produtos.Count} produto(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<ProdutoAlimento>(), "Erro inesperado ao listar produtos.");
            }
        }

        // READ - buscar por nome
        public (List<ProdutoAlimento> produtos, string mensagem) BuscarPorNome(string nome)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome))
                {
                    return (new List<ProdutoAlimento>(), "Nome não pode ser vazio.");
                }

                var produtos = produtoService.BuscarPorNome(nome);
                if (produtos.Count == 0)
                {
                    return (produtos, $"Nenhum produto encontrado com o nome '{nome}'.");
                }
                return (produtos, $"{produtos.Count} produto(s) encontrado(s) com o nome '{nome}'.");
            }
            catch (Exception)
            {
                return (new List<ProdutoAlimento>(), "Erro inesperado ao buscar produtos.");
            }
        }

        // READ - listar produtos com estoque baixo
        public (List<ProdutoAlimento> produtos, string mensagem) ListarProdutosEstoqueBaixo()
        {
            try
            {
                var produtos = produtoService.ListarProdutosEstoqueBaixo();
                if (produtos.Count == 0)
                {
                    return (produtos, "Nenhum produto com estoque baixo.");
                }
                return (produtos, $"{produtos.Count} produto(s) com estoque baixo.");
            }
            catch (Exception)
            {
                return (new List<ProdutoAlimento>(), "Erro inesperado ao listar produtos com estoque baixo.");
            }
        }

        // UPDATE - atualizar dados do produto
        public (bool sucesso, string mensagem) AtualizarProduto(int id, string? nome = null, string? descricao = null,
            float? preco = null, int? estoqueMinimo = null)
        {
            try
            {
                produtoService.AtualizarProduto(id, nome, descricao, preco, estoqueMinimo);
                return (true, "Produto atualizado com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao atualizar produto.");
            }
        }

        // DELETE - remover produto
        public (bool sucesso, string mensagem) DeletarProduto(int id)
        {
            try
            {
                produtoService.DeletarProduto(id);
                return (true, "Produto deletado com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao deletar produto.");
            }
        }

        // ESTOQUE - adicionar quantidade ao estoque
        public (bool sucesso, string mensagem) AdicionarEstoque(int id, int quantidade)
        {
            try
            {
                produtoService.AdicionarEstoque(id, quantidade);
                return (true, "Estoque adicionado com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao adicionar estoque.");
            }
        }

        // ESTOQUE - reduzir quantidade do estoque
        public (bool sucesso, string mensagem) ReduzirEstoque(int id, int quantidade)
        {
            try
            {
                produtoService.ReduzirEstoque(id, quantidade);
                return (true, "Estoque reduzido com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao reduzir estoque.");
            }
        }

        // ESTOQUE - verificar disponibilidade
        public (bool disponivel, string mensagem) VerificarDisponibilidade(int id, int quantidade)
        {
            try
            {
                if (quantidade <= 0)
                {
                    return (false, "Quantidade deve ser maior que zero.");
                }

                var disponivel = produtoService.VerificarDisponibilidade(id, quantidade);
                if (!disponivel)
                {
                    return (false, "Produto sem estoque suficiente ou não encontrado.");
                }
                return (true, "Produto disponível em estoque.");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao verificar disponibilidade.");
            }
        }
    }
}
