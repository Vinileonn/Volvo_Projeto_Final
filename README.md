# CineFlow API

Projeto final de Desenvolvimento Web com .NET. Esta API gerencia cinemas, salas, filmes, sessoes, ingressos, pedidos de alimento e operacoes administrativas.

## Tecnologias
- .NET 10 (Web API)
- C#
- SQL Server
- Entity Framework Core
- Swagger (OpenAPI)

## Diagrama do banco de dados
> Substitua a imagem abaixo pelo diagrama do seu banco.

![Diagrama do banco](docs/diagramas/cineflow-db.png)

## Como executar
1. Configure a connection string no arquivo `cinecore/appsettings.json` (chave `ConnectionStrings:CineFlow`).
2. Crie o banco e as migracoes com EF Core:

```bash
cd cinecore
dotnet ef migrations add InitialCreate
dotnet ef database update
```

3. Rode a API:

```bash
dotnet run
```

4. Acesse o Swagger em `/swagger`.

## Exemplos de chamadas
Cartaz com periodo e filtro de disponibilidade:

```http
GET /api/Relatorio/cartaz?inicio=2026-02-09&fim=2026-02-16&disponiveis=true
```

Ocupacao por sala no periodo:

```http
GET /api/Relatorio/salas/ocupacao?inicio=2026-02-01&fim=2026-02-29
```

## Seed basico
- Um administrador padrao e criado ao iniciar a API usando as credenciais de `Admin:Email` e `Admin:Senha` no `appsettings.json`.

## Estrutura (resumo)
- `cinecore/controladores`: controllers da API
- `cinecore/servicos`: regras de negocio
- `cinecore/modelos`: entidades do dominio
- `cinecore/DTOs`: contratos de entrada/saida
- `cinecore/dados`: DbContext e configuracoes do EF Core
