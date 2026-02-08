using cinecore.servicos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Registra serviços de negócio como Singleton para manter os dados em memória durante a execução
builder.Services.AddSingleton<FilmeServico>();
builder.Services.AddSingleton<SessaoServico>();
builder.Services.AddSingleton<SalaServico>();
builder.Services.AddSingleton<CinemaServico>();
builder.Services.AddSingleton<FuncionarioServico>();
builder.Services.AddSingleton<AluguelSalaServico>();
builder.Services.AddSingleton<UsuarioServico>();
builder.Services.AddSingleton<AutenticacaoServico>();
builder.Services.AddSingleton<IngressoServico>();
builder.Services.AddSingleton<LimpezaServico>();
builder.Services.AddSingleton<ProdutoAlimentoServico>();
builder.Services.AddSingleton<PedidoAlimentoServico>();
builder.Services.AddSingleton<RelatorioServico>();

var app = builder.Build();

// Inicializa o administrador padrão com credenciais do configuration
var usuarioServico = app.Services.GetRequiredService<UsuarioServico>();
var adminEmail = builder.Configuration["Admin:Email"]!;
var adminSenha = builder.Configuration["Admin:Senha"]!;
usuarioServico.InicializarAdministrador(adminEmail, adminSenha);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
