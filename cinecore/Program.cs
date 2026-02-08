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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
