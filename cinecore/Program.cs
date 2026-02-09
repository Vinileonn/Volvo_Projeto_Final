using cinecore.dados;
using cinecore.servicos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CineFlowContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("CineFlow"),
		sqlOptions => sqlOptions.EnableRetryOnFailure(
			maxRetryCount: 5,
			maxRetryDelay: TimeSpan.FromSeconds(10),
			errorNumbersToAdd: null)));

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

// Cria/aplica migrations do banco de dados automaticamente
using (var scope = app.Services.CreateScope())
{
	try
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<CineFlowContext>();
		dbContext.Database.Migrate();
	}
	catch (Exception ex)
	{
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "Erro ao aplicar migrations do banco de dados");
		throw;
	}
}

// Inicializa o administrador padrão com credenciais do configuration
using (var scope = app.Services.CreateScope())
{
	try
	{
		var usuarioServico = scope.ServiceProvider.GetRequiredService<UsuarioServico>();
		var adminEmail = builder.Configuration["Admin:Email"]!;
		var adminSenha = builder.Configuration["Admin:Senha"]!;
		usuarioServico.InicializarAdministrador(adminEmail, adminSenha);
	}
	catch (Exception ex)
	{
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
		logger.LogWarning(ex, "Aviso ao inicializar administrador padrão");
	}
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
