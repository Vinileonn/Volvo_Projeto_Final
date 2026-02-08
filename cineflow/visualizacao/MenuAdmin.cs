using cineflow.controladores;
using cineflow.modelos.UsuarioModelo;
using cineflow.utilitarios;

namespace cineflow.menus
{
    public class MenuAdmin
    {
        private readonly AdministradorControlador administradorControlador;
        private readonly AutenticacaoControlador autenticacaoControlador;
        
        private readonly MenuFilmes menuFilmes;
        private readonly MenuSalas menuSalas;
        private readonly MenuSessoes menuSessoes;
        private readonly MenuProdutos menuProdutos;
        private readonly MenuCinemas menuCinemas;
        private readonly MenuFuncionarios menuFuncionarios;
        private readonly MenuEscalasLimpeza menuEscalasLimpeza;
        private readonly MenuPedidos menuPedidos;
        private readonly MenuRelatorios menuRelatorios;

        public MenuAdmin(
            AdministradorControlador administradorControlador,
            AutenticacaoControlador autenticacaoControlador)
        {
            this.administradorControlador = administradorControlador;
            this.autenticacaoControlador = autenticacaoControlador;
            
            this.menuFilmes = new MenuFilmes(administradorControlador);
            this.menuSalas = new MenuSalas(administradorControlador);
            this.menuSessoes = new MenuSessoes(administradorControlador);
            this.menuProdutos = new MenuProdutos(administradorControlador);
            this.menuCinemas = new MenuCinemas(administradorControlador);
            this.menuFuncionarios = new MenuFuncionarios(administradorControlador);
            this.menuEscalasLimpeza = new MenuEscalasLimpeza(administradorControlador);
            this.menuPedidos = new MenuPedidos(administradorControlador);
            this.menuRelatorios = new MenuRelatorios(administradorControlador);
        }

        public void Executar(Administrador admin)
        {
            while (true)
            {
                MenuHelper.LimparConsole();
                MenuHelper.MostrarTitulo($"Menu Admin - {admin.Nome}");
                MenuHelper.MostrarOpcoes(
                    "Gestao de Filmes",
                    "Gestao de Salas",
                    "Gestao de Sessoes",
                    "Gestao de Produtos",
                    "Gestao de Cinemas",
                    "Gestao de Funcionarios",
                    "Gestao de Escalas de Limpeza",
                    "Gestao de Pedidos",
                    "Relatorios e Estatisticas",
                    "Alterar Senha",
                    "Logout");

                var opcao = MenuHelper.LerOpcaoInteira(0, 11);

                switch (opcao)
                {
                    case 1:
                        menuFilmes.Executar();
                        break;
                    case 2:
                        menuSalas.Executar();
                        break;
                    case 3:
                        menuSessoes.Executar();
                        break;
                    case 4:
                        menuProdutos.Executar();
                        break;
                    case 5:
                        menuCinemas.Executar();
                        break;
                    case 6:
                        menuFuncionarios.Executar();
                        break;
                    case 7:
                        menuEscalasLimpeza.Executar();
                        break;
                    case 8:
                        menuPedidos.Executar();
                        break;
                    case 9:
                        menuRelatorios.Executar();
                        break;
                    case 10:
                        AlterarSenha(admin);
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void AlterarSenha(Administrador admin)
        {
            MenuHelper.LimparConsole();
            MenuHelper.MostrarTitulo("Alterar Senha");

            try
            {
                var senhaAtual = MenuHelper.LerTextoNaoVazio("Senha Atual: ");
                var novaSenha = MenuHelper.LerTextoNaoVazio("Nova Senha: ");
                var confirmaSenha = MenuHelper.LerTextoNaoVazio("Confirmar Nova Senha: ");

                if (novaSenha != confirmaSenha)
                {
                    MenuHelper.ExibirMensagem("Senhas nao conferem.");
                    MenuHelper.Pausar();
                    return;
                }

                var (sucesso, mensagem) = autenticacaoControlador.AlterarSenha(admin.Id, senhaAtual, novaSenha);
                MenuHelper.ExibirMensagem(mensagem);
            }
            catch (Exception ex)
            {
                MenuHelper.ExibirMensagem($"Erro: {ex.Message}");
            }

            MenuHelper.Pausar();
        }
    }
}
