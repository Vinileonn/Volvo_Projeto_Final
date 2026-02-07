// ANTIGO:
// // Veja https://aka.ms/new-console-template para mais informacoes
// Console.WriteLine("Ola, Mundo!");

using cineflow.enumeracoes;
using cineflow.modelos;
using cineflow.modelos.UsuarioModelo;
using cineflow.servicos;
using System.Linq;

// Fluxo simples para demonstrar cinema, sala com especiais e preco final.
var cinemaServico = new CinemaServico();
var salaServico = new SalaServico();
var filmeServico = new FilmeServico();
var sessaoServico = new SessaoServico();
var ingressoServico = new IngressoServico();
var usuarioServico = new UsuarioServico();

var cinema = new Cinema(0, "Cinema Centro", "Rua Principal, 123");
cinemaServico.CriarCinema(cinema);

// Sala XD com 2 assentos PCD e 2 assentos casal (2 lugares cada).
var sala = new Sala(0, "Sala 1", 30, cinema, TipoSala.XD, quantidadeAssentosCasal: 2, quantidadeAssentosPCD: 2);
salaServico.CriarSala(sala);

// Filme 3D para aplicar adicional do 3D no preco final.
var filme = new Filme(0, "Aventura 3D", 120, "Aventura", new DateTime(2024, 1, 1), eh3D: true);
filmeServico.CriarFilme(filme);

// Preco base da sessao, adicionais aplicados pelo SessaoServico.
var sessao = new Sessao(0, DateTime.Now.AddHours(2), 25f, filme, sala);
sessaoServico.CriarSessao(sessao);

var cliente = new Cliente(0, "Ana", "ana@email.com", "123", "12345678900", "9999-0000", "Rua B", new DateTime(2000, 1, 1));
usuarioServico.RegistrarCliente(cliente);

// Escolhe um assento casal se existir; senao, usa o primeiro disponivel.
Assento? assentoEscolhido = null;
foreach (var assento in sala.Assentos)
{
	if (assento.Tipo == TipoAssento.Casal)
	{
		assentoEscolhido = assento;
		break;
	}
}

assentoEscolhido ??= sala.Assentos.First();

// Pagamento em dinheiro acima do total para gerar troco.
var ingresso = ingressoServico.VenderInteira(sessao, cliente, assentoEscolhido.Fila, assentoEscolhido.Numero, FormaPagamento.Dinheiro, 100m);

Console.WriteLine(cinema.ToString());
Console.WriteLine(sala.ToString());
Console.WriteLine(filme.ToString());
Console.WriteLine(sessao.ToString());
Console.WriteLine(ingresso.ToString());
Console.WriteLine(sala.VisualizarDisposicao());

// Fluxo 2: assento PCD + meia entrada.
var salaPCD = new Sala(0, "Sala PCD", 20, cinema, TipoSala.Normal, quantidadeAssentosCasal: 0, quantidadeAssentosPCD: 2);
salaServico.CriarSala(salaPCD);

var filme2D = new Filme(0, "Drama", 90, "Drama", new DateTime(2023, 1, 1), eh3D: false);
filmeServico.CriarFilme(filme2D);

var sessaoPCD = new Sessao(0, DateTime.Now.AddHours(4), 20f, filme2D, salaPCD);
sessaoServico.CriarSessao(sessaoPCD);

Assento? assentoPCD = null;
foreach (var assento in salaPCD.Assentos)
{
	if (assento.Tipo == TipoAssento.PCD)
	{
		assentoPCD = assento;
		break;
	}
}

assentoPCD ??= salaPCD.Assentos.First();

// Pagamento por PIX para meia entrada.
var ingressoMeia = ingressoServico.VenderMeia(sessaoPCD, cliente, assentoPCD.Fila, assentoPCD.Numero, "Estudante", FormaPagamento.Pix);

Console.WriteLine(sessaoPCD.ToString());
Console.WriteLine(ingressoMeia.ToString());
Console.WriteLine(salaPCD.VisualizarDisposicao());





