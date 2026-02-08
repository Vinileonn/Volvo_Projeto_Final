using cineflow.controladores;
using cineflow.servicos;
using cineflow.modelos;
using cineflow.modelos.UsuarioModelo;
using cineflow.enumeracoes;
using cineflow.menus;
using cineflow.visualizacao;
using cineflow.utilitarios;

// Inicializacao de servicos (ordem conforme o plano).
var cinemaServico = new CinemaServico();
var filmeServico = new FilmeServico();
var salaServico = new SalaServico();
var sessaoServico = new SessaoServico();
var aluguelSalaServico = new AluguelSalaServico(sessaoServico);
var produtoAlimentoServico = new ProdutoAlimentoServico();
var pedidoAlimentoServico = new PedidoAlimentoServico(produtoAlimentoServico);
var ingressoServico = new IngressoServico();
var usuarioServico = new UsuarioServico();
var autenticacaoServico = new AutenticacaoServico(usuarioServico);
var funcionarioServico = new FuncionarioServico();
var limpezaServico = new LimpezaServico();
var relatorioServico = new RelatorioServico(
	ingressoServico,
	sessaoServico,
	produtoAlimentoServico,
	pedidoAlimentoServico);

// Inicializacao de controladores (ordem conforme o plano).
var filmeControlador = new FilmeControlador(filmeServico);
var salaControlador = new SalaControlador(salaServico);
var sessaoControlador = new SessaoControlador(sessaoServico);
var produtoControlador = new ProdutoControlador(produtoAlimentoServico);
var pedidoControlador = new PedidoControlador(pedidoAlimentoServico);
var cinemaControlador = new CinemaControlador(cinemaServico);
var funcionarioControlador = new FuncionarioControlador(funcionarioServico);
var limpezaControlador = new LimpezaControlador(limpezaServico);
var relatorioControlador = new RelatorioControlador(relatorioServico);
var usuarioControlador = new UsuarioControlador(usuarioServico);
var aluguelSalaControlador = new AluguelSalaControlador(aluguelSalaServico);
var clienteControlador = new ClienteControlador(
	filmeServico,
	sessaoServico,
	ingressoServico,
	pedidoAlimentoServico,
	produtoAlimentoServico);
var autenticacaoControlador = new AutenticacaoControlador(autenticacaoServico, usuarioServico);
var administradorControlador = new AdministradorControlador(
	filmeControlador,
	salaControlador,
	sessaoControlador,
	produtoControlador,
	pedidoControlador,
	relatorioControlador,
	cinemaControlador,
	funcionarioControlador,
	limpezaControlador,
	usuarioControlador);

// ═══════════════════════════════════════════════════════════════════════════════
// CRIAR DADOS DE TESTE INICIAIS
// ═══════════════════════════════════════════════════════════════════════════════
CriarDadosDeTeste();

var menuPrincipal = new MenuPrincipal(clienteControlador, autenticacaoControlador, produtoControlador);
var menuCliente = new MenuCliente(clienteControlador, pedidoControlador, autenticacaoControlador, salaControlador, aluguelSalaControlador);
var menuAdmin = new MenuAdmin(administradorControlador, autenticacaoControlador, aluguelSalaControlador);

while (true)
{
    var resultado = menuPrincipal.Executar();
    if (resultado.Encerrar)
    {
        break;
    }

    if (resultado.UsuarioLogado is Cliente cliente)
    {
        menuCliente.Executar(cliente, resultado.Carrinho);
        continue;
    }

    if (resultado.UsuarioLogado is Administrador admin)
    {
        menuAdmin.Executar(admin);
    }
}

void CriarDadosDeTeste()
{
    Console.WriteLine("Inicializando dados de teste...");

    // 1. CRIAR ADMINISTRADOR PADRÃO
    var admin = new Administrador(1, "Administrador do Sistema");
    usuarioControlador.AdicionarUsuario(admin);

    // 2. CRIAR CINEMA
    var cinema = new Cinema(1, "CineFlow Plaza", "Av. Principal, 123");
    cinemaControlador.CriarCinema(cinema);

    // 3. CRIAR SALAS
    var sala1 = new Sala(1, "Sala 1", 50, cinema, TipoSala.Normal, 4, 2);
    salaControlador.CriarSala(sala1);
    salaControlador.GerarAssentosParaSala(1);

    var sala2 = new Sala(2, "Sala 2", 80, cinema, TipoSala.XD, 6, 4);
    salaControlador.CriarSala(sala2);
    salaControlador.GerarAssentosParaSala(2);

    var sala3 = new Sala(3, "Sala 3", 120, cinema, TipoSala.VIP, 8, 6);
    salaControlador.CriarSala(sala3);
    salaControlador.GerarAssentosParaSala(3);

    var sala4 = new Sala(4, "Sala 4", 60, cinema, TipoSala.QuatroD, 4, 2);
    salaControlador.CriarSala(sala4);
    salaControlador.GerarAssentosParaSala(4);

    // 4. CRIAR FILMES
    var filme1 = new Filme(1, "Avatar: O Caminho da Agua", 192, "Ficcao Cientifica", new DateTime(2022, 12, 16), true, ClassificacaoIndicativa.Doze);
    filmeControlador.CriarFilme(filme1);

    var filme2 = new Filme(2, "Vingadores: Ultimato", 181, "Acao/Aventura", new DateTime(2019, 4, 26), false, ClassificacaoIndicativa.Doze);
    filmeControlador.CriarFilme(filme2);

    var filme3 = new Filme(3, "Interestelar", 169, "Ficcao Cientifica", new DateTime(2014, 11, 7), false, ClassificacaoIndicativa.Dez);
    filmeControlador.CriarFilme(filme3);

    var filme4 = new Filme(4, "O Poderoso Chefao", 175, "Drama/Crime", new DateTime(1972, 3, 24), false, ClassificacaoIndicativa.Dezesseis);
    filmeControlador.CriarFilme(filme4);

    var filme5 = new Filme(5, "Homem-Aranha: Atraves do Aranhaverso", 140, "Animacao/Acao", new DateTime(2023, 6, 1), true, ClassificacaoIndicativa.Livre);
    filmeControlador.CriarFilme(filme5);

    // 5. CRIAR PRODUTOS
    produtoControlador.CriarProduto(new ProdutoAlimento(1, "Pipoca Pequena", "Pipoca salgada 50g", CategoriaProduto.Alimento, 8.00f, 100, 20));
    produtoControlador.CriarProduto(new ProdutoAlimento(2, "Pipoca Média", "Pipoca salgada 100g", CategoriaProduto.Alimento, 12.00f, 100, 20));
    produtoControlador.CriarProduto(new ProdutoAlimento(3, "Pipoca Grande", "Pipoca salgada 150g", CategoriaProduto.Alimento, 16.00f, 100, 20));
    produtoControlador.CriarProduto(new ProdutoAlimento(4, "Refrigerante Pequeno", "Refrigerante 300ml", CategoriaProduto.Bebida, 6.00f, 150, 30));
    produtoControlador.CriarProduto(new ProdutoAlimento(5, "Refrigerante Médio", "Refrigerante 500ml", CategoriaProduto.Bebida, 9.00f, 150, 30));
    produtoControlador.CriarProduto(new ProdutoAlimento(6, "Refrigerante Grande", "Refrigerante 700ml", CategoriaProduto.Bebida, 12.00f, 150, 30));
    produtoControlador.CriarProduto(new ProdutoAlimento(7, "Combo Casal", "Pipoca Grande + 2 Refrigerantes Médios", CategoriaProduto.Alimento, 32.00f, 50, 10));
    produtoControlador.CriarProduto(new ProdutoAlimento(8, "Combo Individual", "Pipoca Média + Refrigerante Médio", CategoriaProduto.Alimento, 18.00f, 80, 15));
    produtoControlador.CriarProduto(new ProdutoAlimento(9, "Combo Família", "2 Pipocas Grandes + 4 Refrigerantes Grandes", CategoriaProduto.Alimento, 60.00f, 30, 5));
    produtoControlador.CriarProduto(new ProdutoAlimento(10, "Chocolate", "Barra de chocolate ao leite", CategoriaProduto.Doce, 5.00f, 200, 40));
    produtoControlador.CriarProduto(new ProdutoAlimento(11, "Bala de Goma", "Pacote de balas sortidas", CategoriaProduto.Doce, 4.00f, 200, 40));
    produtoControlador.CriarProduto(new ProdutoAlimento(12, "Nachos com Queijo", "Nachos crocantes com molho de queijo", CategoriaProduto.Alimento, 15.00f, 60, 12));
    produtoControlador.CriarProduto(new ProdutoAlimento(13, "Balde Avatar", "Balde tematico do filme Avatar", CategoriaProduto.Tematico, 28.00f, 40, 10, true, "Avatar: O Caminho da Agua"));
    produtoControlador.CriarProduto(new ProdutoAlimento(14, "Poster Interestelar", "Poster colecionavel", CategoriaProduto.Poster, 20.00f, 30, 5));
    produtoControlador.CriarProduto(new ProdutoAlimento(15, "Brinde CineFlow", "Brinde surpresa", CategoriaProduto.Brinde, 0.00f, 100, 10, false, null, true, false));
    produtoControlador.CriarProduto(new ProdutoAlimento(16, "Cortesia Pre-Estreia", "Cortesia exclusiva", CategoriaProduto.Cortesia, 0.00f, 200, 10, false, null, true, true));

    // 5.1 CRIAR FUNCIONARIOS
    funcionarioControlador.CriarFuncionario(new Funcionario(0, "Gerente Geral", CargoFuncionario.Gerente, cinema));
    funcionarioControlador.CriarFuncionario(new Funcionario(0, "Garcom VIP", CargoFuncionario.Garcom, cinema));

    // 6. CRIAR SESSÕES (próximos 7 dias)
    int sessaoId = 1;
    DateTime dataBase = new DateTime(2026, 2, 7);
    
    for (int dia = 0; dia < 7; dia++)
    {
        DateTime data = dataBase.AddDays(dia);
        
        // Avatar (Sala 3) - 14h e 20h
        sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(data.Year, data.Month, data.Day, 14, 0, 0), 25.00f, filme1, sala3, TipoSessao.Regular, null, null, IdiomaSessao.Dublado));
        sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(data.Year, data.Month, data.Day, 20, 0, 0), 30.00f, filme1, sala3, TipoSessao.Regular, null, null, IdiomaSessao.Legendado));
        
        // Vingadores (Sala 2) - 15h e 21h
        sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(data.Year, data.Month, data.Day, 15, 0, 0), 22.00f, filme2, sala2, TipoSessao.Regular, null, null, IdiomaSessao.Dublado));
        sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(data.Year, data.Month, data.Day, 21, 0, 0), 25.00f, filme2, sala2, TipoSessao.Regular, null, null, IdiomaSessao.Legendado));
        
        // Interestelar (Sala 1) - 16h
        sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(data.Year, data.Month, data.Day, 16, 0, 0), 18.00f, filme3, sala1, TipoSessao.Regular, null, null, IdiomaSessao.Dublado));
        
        // O Poderoso Chefão (Sala 1) - 19h
        sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(data.Year, data.Month, data.Day, 19, 0, 0), 20.00f, filme4, sala1, TipoSessao.Regular, null, null, IdiomaSessao.Legendado));
        
        // Homem-Aranha (Sala 2) - 13h e 18h
        sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(data.Year, data.Month, data.Day, 13, 0, 0), 24.00f, filme5, sala2, TipoSessao.Regular, null, null, IdiomaSessao.Dublado));
        sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(data.Year, data.Month, data.Day, 18, 0, 0), 26.00f, filme5, sala2, TipoSessao.Regular, null, null, IdiomaSessao.Legendado));
    }

    // Sessoes especiais
    sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(dataBase.Year, dataBase.Month, dataBase.Day, 10, 0, 0), 35.00f, filme1, sala3, TipoSessao.PreEstreia, null, null, IdiomaSessao.Legendado));
    sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(dataBase.Year, dataBase.Month, dataBase.Day, 9, 0, 0), 18.00f, filme3, sala1, TipoSessao.EspecialBebe, null, null, IdiomaSessao.Dublado));
    sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(dataBase.Year, dataBase.Month, dataBase.Day, 11, 0, 0), 20.00f, filme4, sala4, TipoSessao.EspecialPet, null, null, IdiomaSessao.Legendado));
    sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(dataBase.Year, dataBase.Month, dataBase.Day, 8, 30, 0), 16.00f, filme5, sala1, TipoSessao.Matine, null, null, IdiomaSessao.Dublado));
    sessaoControlador.CriarSessao(new Sessao(sessaoId++, new DateTime(dataBase.Year, dataBase.Month, dataBase.Day, 22, 0, 0), 28.00f, filme2, sala2, TipoSessao.Evento, "Maratona Vingadores", "Parceiro Geek Club", IdiomaSessao.Legendado));

    Console.WriteLine("✓ Dados de teste criados com sucesso!");
    Console.WriteLine($"  • Admin: Administrador@cinema.com / administrador123");
    Console.WriteLine($"  • 1 Cinema, 4 Salas, 5 Filmes, 16 Produtos, {sessaoId - 1} Sessoes");
    Console.WriteLine();
}
