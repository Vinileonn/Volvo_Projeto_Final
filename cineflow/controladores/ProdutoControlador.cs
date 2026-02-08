using cineflow.modelos;
using cineflow.servicos;
using cineflow.excecoes;
using cineflow.enumeracoes;

namespace cineflow.controladores
{
    public class ProdutoControlador
    {
        private readonly ProdutoAlimentoServico produtoService;

        public ProdutoControlador(ProdutoAlimentoServico produtoService)
        {
            this.produtoService = produtoService;
        }
        public (bool sucesso, string mensagem) CriarProduto(ProdutoAlimento produto)
        {
            try
            {
                produtoService.CriarProduto(produto);
                return (true, $"Produto '{produto.Nome}' cadastrado com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao cadastrar produto.");
            }
        }
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

        public (List<string> alertas, string mensagem) ListarAlertasEstoque()
        {
            try
            {
                var alertas = produtoService.ListarAlertasEstoque();
                if (alertas.Count == 0)
                {
                    return (alertas, "Nenhum alerta de estoque.");
                }
                return (alertas, $"{alertas.Count} alerta(s) de estoque.");
            }
            catch (Exception)
            {
                return (new List<string>(), "Erro inesperado ao listar alertas.");
            }
        }
        public (bool sucesso, string mensagem) AtualizarProduto(int id, string? nome = null, string? descricao = null,
            float? preco = null, int? estoqueMinimo = null, bool? ehTematico = null, string? temaFilme = null,
            bool? ehCortesia = null, bool? exclusivoPreEstreia = null, CategoriaProduto? categoria = null)
        {
            try
            {
                produtoService.AtualizarProduto(id, nome, descricao, preco, estoqueMinimo, ehTematico, temaFilme,
                    ehCortesia, exclusivoPreEstreia, categoria);
                return (true, "Produto atualizado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao atualizar produto.");
            }
        }
        public (bool sucesso, string mensagem) DeletarProduto(int id)
        {
            try
            {
                produtoService.DeletarProduto(id);
                return (true, "Produto deletado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
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
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosExcecao ex)
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
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosExcecao ex)
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





