using cineflow.controladores;
using cineflow.modelos;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuPedidos
    {
        private readonly AdministradorControlador administradorControlador;

        public MenuPedidos(AdministradorControlador administradorControlador)
        {
            this.administradorControlador = administradorControlador;
        }

        public void Executar()
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo("Gestao de Pedidos");
                MenuHelper.MostrarOpcoes(
                    "Listar Todos os Pedidos",
                    "Obter Detalhes de Pedido",
                    "Cancelar Pedido");

                var opcao = MenuHelper.LerOpcaoInteira(0, 3);

                switch (opcao)
                {
                    case 1:
                        ListarTodosPedidos();
                        break;
                    case 2:
                        ObterDetalhesPedido();
                        break;
                    case 3:
                        CancelarPedido();
                        break;
                    case 0:
                        return;
                }
            }
        }

        // LER - 
        private void ListarTodosPedidos()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Listar Todos os Pedidos");

            var (pedidos, mensagem) = administradorControlador.PedidoControlador.ListarPedidos();
            MenuHelper.ExibirMensagem(mensagem);

            if (pedidos.Count > 0)
            {
                ExibirPedidosTabela(pedidos);
            }

            MenuHelper.Pausar();
        }

        // LER - 
        private void ObterDetalhesPedido()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Obter Detalhes de Pedido");

            var id = MenuHelper.LerInteiro("ID do Pedido: ", 1, 999999);
            var (pedido, mensagem) = administradorControlador.PedidoControlador.ObterPedido(id);

            MenuHelper.ExibirMensagem(mensagem);

            if (pedido != null)
            {
                Console.WriteLine($"\nID: {pedido.Id}");
                Console.WriteLine($"Data do Pedido: {pedido.DataPedido:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Itens: {pedido.Itens.Count}");

                if (pedido.Itens.Count > 0)
                {
                    Console.WriteLine("\nItens do Pedido:");
                    foreach (var item in pedido.Itens)
                    {
                        if (item.Produto != null)
                        {
                            Console.WriteLine($"  - {item.Produto.Nome}: {item.Quantidade} x {FormatadorMoeda.Formatar(item.Preco)} = {FormatadorMoeda.Formatar(item.Quantidade * item.Preco)}");
                        }
                    }
                }

                var (total, totalMensagem) = administradorControlador.PedidoControlador.CalcularTotal(id);
                Console.WriteLine($"\nTotal: {FormatadorMoeda.Formatar(total)}");
                if (pedido.ValorDesconto > 0)
                {
                    Console.WriteLine($"Desconto: {FormatadorMoeda.Formatar(pedido.ValorDesconto)}");
                    if (!string.IsNullOrWhiteSpace(pedido.MotivoDesconto))
                    {
                        Console.WriteLine($"Motivo: {pedido.MotivoDesconto}");
                    }
                }
                if (pedido.PontosUsados > 0)
                {
                    Console.WriteLine($"Pontos usados: {pedido.PontosUsados}");
                }
                if (pedido.PontosGerados > 0)
                {
                    Console.WriteLine($"Pontos gerados: {pedido.PontosGerados}");
                }
            }

            MenuHelper.Pausar();
        }
// DELETAR - 
        
        private void CancelarPedido()
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Cancelar Pedido");

            var id = MenuHelper.LerInteiro("ID do Pedido: ", 1, 999999);

            Console.Write("Tem certeza que deseja cancelar? (sim/nao): ");
            var confirmacao = Console.ReadLine();

            if (confirmacao?.ToLower() == "sim")
            {
                var (sucesso, mensagem) = administradorControlador.PedidoControlador.CancelarPedido(id);
                MenuHelper.ExibirMensagem(mensagem);
            }
            else
            {
                MenuHelper.ExibirMensagem("Operacao cancelada.");
            }

            MenuHelper.Pausar();
        }

        private void ExibirPedidosTabela(List<PedidoAlimento> pedidos)
        {
            Console.WriteLine("\n{0,-4} {1,-20} {2,-10}", "ID", "Data", "Itens");
            Console.WriteLine(new string('-', 40));
            foreach (var pedido in pedidos)
            {
                Console.WriteLine("{0,-4} {1,-20} {2,-10}", pedido.Id, pedido.DataPedido.ToString("dd/MM/yyyy HH:mm"), pedido.Itens.Count);
            }
        }
    }
}
