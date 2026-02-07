using cineflow.controladores;

namespace cineflow.controladores
{
    // Renomeado de AdminController para AdministradorControlador.
    public class AdministradorControlador
    {
        public FilmeControlador FilmeControlador { get; }
        public SalaControlador SalaControlador { get; }
        public SessaoControlador SessaoControlador { get; }
        public ProdutoControlador ProdutoControlador { get; }
        public PedidoControlador PedidoControlador { get; }
        public RelatorioControlador RelatorioControlador { get; }
        public CinemaControlador CinemaControlador { get; }
        public FuncionarioControlador FuncionarioControlador { get; }
        public LimpezaControlador LimpezaControlador { get; }

        // ANTIGO:
        // public AdministradorControlador(
        //     FilmeControlador FilmeControlador,
        //     SalaControlador SalaControlador,
        //     SessaoControlador SessaoControlador,
        //     ProdutoControlador ProdutoControlador,
        //     PedidoControlador PedidoControlador,
        //     RelatorioControlador RelatorioControlador)
        // {
        //     FilmeControlador = FilmeControlador;
        //     SalaControlador = SalaControlador;
        //     SessaoControlador = SessaoControlador;
        //     ProdutoControlador = ProdutoControlador;
        //     PedidoControlador = PedidoControlador;
        //     RelatorioControlador = RelatorioControlador;
        // }

        // ANTIGO:
        // public AdministradorControlador(
        //     FilmeControlador FilmeControlador,
        //     SalaControlador SalaControlador,
        //     SessaoControlador SessaoControlador,
        //     ProdutoControlador ProdutoControlador,
        //     PedidoControlador PedidoControlador,
        //     RelatorioControlador RelatorioControlador,
        //     CinemaControlador CinemaControlador)
        // {
        //     FilmeControlador = FilmeControlador;
        //     SalaControlador = SalaControlador;
        //     SessaoControlador = SessaoControlador;
        //     ProdutoControlador = ProdutoControlador;
        //     PedidoControlador = PedidoControlador;
        //     RelatorioControlador = RelatorioControlador;
        //     CinemaControlador = CinemaControlador;
        // }

        public AdministradorControlador(
            FilmeControlador FilmeControlador,
            SalaControlador SalaControlador,
            SessaoControlador SessaoControlador,
            ProdutoControlador ProdutoControlador,
            PedidoControlador PedidoControlador,
            RelatorioControlador RelatorioControlador,
            CinemaControlador CinemaControlador,
            FuncionarioControlador FuncionarioControlador,
            LimpezaControlador LimpezaControlador)
        {
            this.FilmeControlador = FilmeControlador;
            this.SalaControlador = SalaControlador;
            this.SessaoControlador = SessaoControlador;
            this.ProdutoControlador = ProdutoControlador;
            this.PedidoControlador = PedidoControlador;
            this.RelatorioControlador = RelatorioControlador;
            this.CinemaControlador = CinemaControlador;
            this.FuncionarioControlador = FuncionarioControlador;
            this.LimpezaControlador = LimpezaControlador;
        }
    }
}





