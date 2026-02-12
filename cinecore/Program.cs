using cinecore.Data;
using cinecore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo { Title = "cinecore", Version = "v1" });
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Insira 'Bearer {token}' para autenticar."
	});
	options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecuritySchemeReference("Bearer", null, null),
			new List<string>()
		}
	});
});

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
	};
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdministradorOnly", policy => policy.RequireRole("Administrador"));
});

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
