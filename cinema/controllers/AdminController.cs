using cinema.controllers;

namespace cinema.controllers
{
    public class AdminController
    {
        public FilmeController FilmeController { get; }
        public SalaController SalaController { get; }
        public SessaoController SessaoController { get; }
        public ProdutoController ProdutoController { get; }
        public PedidoController PedidoController { get; }
        public RelatorioController RelatorioController { get; }

        public AdminController(
            FilmeController filmeController,
            SalaController salaController,
            SessaoController sessaoController,
            ProdutoController produtoController,
            PedidoController pedidoController,
            RelatorioController relatorioController)
        {
            FilmeController = filmeController;
            SalaController = salaController;
            SessaoController = sessaoController;
            ProdutoController = produtoController;
            PedidoController = pedidoController;
            RelatorioController = relatorioController;
        }
    }
}
