using cinecore.dados;
using cinecore.servicos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CineFlowContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("CineFlow")));

// Registra AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Registra serviços de negócio como Scoped para usar DbContext por request
builder.Services.AddScoped<FilmeServico>();
builder.Services.AddScoped<SessaoServico>();
builder.Services.AddScoped<SalaServico>();
builder.Services.AddScoped<CinemaServico>();
builder.Services.AddScoped<FuncionarioServico>();
builder.Services.AddScoped<AluguelSalaServico>();
builder.Services.AddScoped<UsuarioServico>();
builder.Services.AddScoped<AutenticacaoServico>();
builder.Services.AddScoped<IngressoServico>();
builder.Services.AddScoped<LimpezaServico>();
builder.Services.AddScoped<ProdutoAlimentoServico>();
builder.Services.AddScoped<PedidoAlimentoServico>();
builder.Services.AddScoped<RelatorioServico>();

var app = builder.Build();

// Inicializa o administrador padrão com credenciais do configuration
using (var scope = app.Services.CreateScope())
{
	var usuarioServico = scope.ServiceProvider.GetRequiredService<UsuarioServico>();
	var adminEmail = builder.Configuration["Admin:Email"]!;
	var adminSenha = builder.Configuration["Admin:Senha"]!;
	usuarioServico.InicializarAdministrador(adminEmail, adminSenha);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
