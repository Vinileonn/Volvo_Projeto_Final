# CineFlow - Sistema de Gerenciamento de Cinema

## 📋 Descrição do Projeto

API RESTful completa para gerenciamento de cinema desenvolvida como projeto final da disciplina de Desenvolvimento Web com .NET. O sistema permite gerenciar filmes, salas, sessões, venda de ingressos, alimentos e muito mais.

---

## 🛠️ Tecnologias Utilizadas

- **.NET 10.0**
- **C#**
- **ASP.NET Core Web API**
- **Entity Framework Core 10.0.2**
- **SQL Server**
- **AutoMapper 12.0.1**
- **Swagger/OpenAPI**

---

## 📊 Modelagem do Banco de Dados

### Diagrama Entidade-Relacionamento

```
┌──────────────────┐
│     CINEMA       │
├──────────────────┤
│ Id (PK)          │
│ Nome             │
│ Endereco         │
│ CNPJ             │
│ Telefone         │
└────┬─────────────┘
     │ 1
     │
     │ N
┌────┴─────────────┐           ┌──────────────────┐
│  FUNCIONARIO     │           │      SALA        │
├──────────────────┤           ├──────────────────┤
│ Id (PK)          │           │ Id (PK)          │
│ Nome             │◄──────────┤ Nome             │
│ Email            │         N │ Capacidade       │
│ Cargo            │           │ Tipo             │
│ CinemaId (FK)    │           │ CinemaId (FK)    │
└──────────────────┘           └────┬─────────────┘
                                    │ 1
                                    │
                                    │ N
┌──────────────────┐           ┌────┴─────────────┐
│     FILME        │           │    ASSENTO       │
├──────────────────┤           ├──────────────────┤
│ Id (PK)          │           │ Id (PK)          │
│ Titulo           │           │ Fila             │
│ Duracao          │           │ Numero           │
│ Genero           │           │ Tipo             │
│ AnoLancamento    │           │ Disponivel       │
│ Eh3D             │           │ SalaId (FK)      │
│ Classificacao    │           └──────────────────┘
└────┬─────────────┘
     │ 1
     │
     │ N
┌────┴─────────────┐           ┌──────────────────┐
│     SESSAO       │         1 │    INGRESSO      │
├──────────────────┤◄──────────┤──────────────────┤
│ Id (PK)          │           │ Id (PK)          │
│ DataHorario      │           │ Fila             │
│ PrecoBase        │           │ Numero           │
│ PrecoFinal       │           │ DataCompra       │
│ Tipo             │           │ ValorPago        │
│ Idioma           │           │ FormaPagamento   │
│ FilmeId (FK)     │           │ CheckInRealizado │
│ SalaId (FK)      │           │ PontosUsados     │
└──────────────────┘           │ PontosGerados    │
                               │ SessaoId (FK)    │
                               │ ClienteId (FK)   │
                               │ AssentoId (FK)   │
                               └────┬─────────────┘
                                    │
                                    │
┌──────────────────┐                │
│    USUARIO       │                │
├──────────────────┤                │
│ Id (PK)          │                │
│ Nome             │         N      │
│ Email            │◄───────────────┘
│ Senha            │
│ CPF              │
│ DataNascimento   │
│ TipoUsuario      │ (Discriminator: Cliente, Administrador)
└────┬─────────────┘
     │
     ├─► CLIENTE
     │   ├─ Telefone
     │   ├─ PontosAcumulados
     │   └─ NivelFidelidade
     │
     └─► ADMINISTRADOR

┌──────────────────┐           ┌──────────────────┐
│ PRODUTOALIMENTO  │         N │ PEDIDOALIMENTO   │
├──────────────────┤◄──────────┤──────────────────┤
│ Id (PK)          │           │ Id (PK)          │
│ Nome             │           │ DataPedido       │
│ Preco            │           │ ValorTotal       │
│ Categoria        │           │ ClienteId (FK)   │
│ Estoque          │           └──────────────────┘
│ EstoqueMinimo    │                    │
└──────────────────┘                    │ N
                                        ▼
                               ┌──────────────────┐
                               │ ITEMPEDIDO       │
                               ├──────────────────┤
                               │ Id (PK)          │
                               │ Quantidade       │
                               │ PrecoUnitario    │
                               │ ProdutoId (FK)   │
                               └──────────────────┘

┌──────────────────┐
│  ALUGUELSALA     │
├──────────────────┤
│ Id (PK)          │
│ DataInicio       │
│ DataFim          │
│ ValorTotal       │
│ Status           │
│ SalaId (FK)      │
│ ClienteId (FK)   │
└──────────────────┘

┌──────────────────┐
│  ESCALALIMPEZA   │
├──────────────────┤
│ Id (PK)          │
│ DataHora         │
│ Concluida        │
│ Observacoes      │
│ SalaId (FK)      │
│ FuncionarioId(FK)│
└──────────────────┘
```

### Principais Relacionamentos

- **Cinema** `1:N` **Sala** - Um cinema possui várias salas
- **Cinema** `1:N` **Funcionario** - Um cinema possui vários funcionários
- **Sala** `1:N` **Assento** - Uma sala possui vários assentos
- **Sala** `1:N` **Sessao** - Uma sala pode ter várias sessões
- **Filme** `1:N` **Sessao** - Um filme pode ter várias sessões
- **Sessao** `1:N` **Ingresso** - Uma sessão pode ter vários ingressos vendidos
- **Cliente** `1:N` **Ingresso** - Um cliente pode comprar vários ingressos
- **Cliente** `1:N` **PedidoAlimento** - Um cliente pode fazer vários pedidos
- **PedidoAlimento** `1:N` **ItemPedidoAlimento** - Um pedido possui vários itens
- **ProdutoAlimento** `1:N` **ItemPedidoAlimento** - Um produto pode estar em vários itens de pedido

---

## 🎯 Funcionalidades Implementadas

### Requisitos Obrigatórios

#### ✅ Gestão de Filmes, Salas e Sessões
- CRUD completo de filmes
- CRUD completo de salas
- CRUD completo de sessões
- **Validação de conflito de horários**: Não permite criar sessão se já houver filme rodando na sala

#### ✅ Venda de Ingressos
- Venda de ingresso inteiro
- Venda de meia-entrada
- **Verificação de lotação**: Valida se a sessão não está lotada antes de vender
- Verificação de classificação indicativa

#### ✅ Desafios LINQ/SQL

**1. Cartaz - Filmes nos próximos 7 dias**
```csharp
GET /api/Relatorio/cartaz?inicio={data}&fim={data}&disponiveis={bool}
```
Lista filmes com sessões disponíveis no período especificado (padrão: 7 dias).

**2. Ocupação por Sala**
```csharp
GET /api/Relatorio/salas/ocupacao?inicio={data}&fim={data}
```
Lista salas e sua taxa de ocupação (ingressos vendidos / capacidade total).

### Funcionalidades Extras (Criatividade)

#### 🎭 Sistema Avançado de Cinema
- **Tipos de Sala**: Normal, XD, VIP, 4D
- **Tipos de Sessão**: Regular, Matinê, Pré-estreia, Evento, Especial (Bebê/Pet)
- **Tipos de Assento**: Normal, Casal, PCD, Preferencial
- **Idioma**: Dublado, Legendado, Nacional

#### 👤 Sistema de Usuários
- Autenticação de administradores e clientes
- Perfis com dados completos
- Sistema de fidelidade com pontos
- Descontos de aniversário

#### 🎫 Sistema de Ingressos Avançado
- Reserva antecipada (com taxa)
- Check-in eletrônico
- Sistema de pontos (usar e ganhar)
- Cupons de desconto de parceiros
- Cálculo automático de troco detalhado

#### 🍿 Sistema de Alimentos
- Catálogo de produtos (Pipoca, Bebida, Combo, Doce)
- Controle de estoque
- Pedidos vinculados a clientes
- Alertas de estoque baixo

#### 🏢 Gestão Operacional
- Aluguel de salas para eventos
- Escala de limpeza de salas
- Gestão de funcionários (Gerente, Garçom, Faxineiro, Bilheteiro)
- Validações de requisitos (ex: Sala VIP exige garçom)

#### 📊 Relatórios Completos
- Total de ingressos vendidos
- Receita total (ingressos + alimentos)
- Ingressos por filme
- Sessões com maior ocupação
- Produtos mais vendidos
- Vendas por período
- Taxa média de ocupação

---

## 🏗️ Arquitetura do Projeto

### Estrutura de Pastas

```
cinecore/
├── controladores/        # Controllers da API (camada de apresentação)
├── servicos/            # Lógica de negócio
├── modelos/             # Entidades do banco de dados
├── dados/               # DbContext e configurações do EF Core
├── DTOs/                # Data Transfer Objects (entrada/saída da API)
├── Mappings/            # Perfis do AutoMapper
├── enums/               # Enumerações do domínio
├── excecoes/            # Exceções customizadas
├── utilitarios/         # Classes auxiliares
└── Migrations/          # Migrações do Entity Framework
```

### Padrões e Boas Práticas

- ✅ **Injeção de Dependência**: Todos os serviços registrados como Scoped
- ✅ **Separation of Concerns**: Controllers, Services, Repositories
- ✅ **DTOs**: Separação entre modelos de domínio e API
- ✅ **AutoMapper**: Mapeamento automático entre DTOs e entidades
- ✅ **Exceções Customizadas**: Tratamento de erros padronizado
- ✅ **Status Codes HTTP**: Uso correto (200, 201, 400, 404, 409, 500)
- ✅ **Validações**: Data Annotations e validações de negócio
- ✅ **Migrations Automáticas**: Aplicadas automaticamente ao iniciar

---

## 🚀 Como Executar

### Pré-requisitos

- .NET 10.0 SDK
- SQL Server (LocalDB ou instância completa)
- Visual Studio 2022 ou VS Code

### Passos

1. **Clone o repositório**
   ```bash
   git clone <seu-repositorio>
   cd projetofinal/cinecore
   ```

2. **Configure a string de conexão**
   
   Edite `appsettings.json` ou `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "CineFlow": "Server=(localdb)\\mssqllocaldb;Database=CineFlowDb;Trusted_Connection=true;TrustServerCertificate=true"
     }
   }
   ```

3. **Execute as migrations** (se necessário)
   ```bash
   dotnet ef database update
   ```
   
   > **Nota**: As migrations são aplicadas automaticamente ao iniciar a aplicação.

4. **Execute o projeto**
   ```bash
   dotnet run
   ```

5. **Acesse o Swagger**
   ```
   https://localhost:7xxx/swagger
   ```

---

## 📡 Principais Endpoints da API

### Filmes
- `POST /api/Filme/Criar` - Criar filme
- `GET /api/Filme` - Listar filmes
- `GET /api/Filme/Obter/{id}` - Obter filme por ID
- `PUT /api/Filme/Atualizar/{id}` - Atualizar filme
- `DELETE /api/Filme/Deletar/{id}` - Deletar filme

### Sessões
- `POST /api/Sessao/Criar` - Criar sessão (com validação de conflito)
- `GET /api/Sessao` - Listar sessões
- `GET /api/Sessao/Obter/{id}` - Obter sessão por ID
- `PUT /api/Sessao/Atualizar/{id}` - Atualizar sessão
- `DELETE /api/Sessao/Deletar/{id}` - Deletar sessão

### Ingressos
- `POST /api/Ingresso/VenderInteira` - Vender ingresso inteiro
- `POST /api/Ingresso/VenderMeia` - Vender meia-entrada
- `POST /api/Ingresso/CheckIn/{id}` - Realizar check-in
- `DELETE /api/Ingresso/Cancelar/{id}` - Cancelar ingresso

### Relatórios (Desafios)
- `GET /api/Relatorio/cartaz` - **Cartaz (filmes próximos 7 dias)**
- `GET /api/Relatorio/salas/ocupacao` - **Taxa de ocupação por sala**
- `GET /api/Relatorio/ingressos/total` - Total de ingressos vendidos
- `GET /api/Relatorio/ingressos/receita` - Receita de ingressos
- `GET /api/Relatorio/sessoes/maior-ocupacao` - Sessões mais lotadas

### Outros
- Salas, Cinemas, Usuários, Funcionários, Produtos, Pedidos, Aluguel de Salas, Limpeza

---

## 🧪 Exemplos de Uso

### Criar uma Sessão

```http
POST /api/Sessao/Criar
Content-Type: application/json

{
  "dataHorario": "2026-02-15T20:00:00",
  "precoBase": 25.00,
  "tipo": "Regular",
  "idioma": "Dublado",
  "filmeId": 1,
  "salaId": 1
}
```

### Vender Ingresso

```http
POST /api/Ingresso/VenderInteira
Content-Type: application/json

{
  "sessaoId": 1,
  "clienteId": 1,
  "fila": "A",
  "numero": 10,
  "formaPagamento": "Credito",
  "valorPago": 30.00,
  "reservaAntecipada": false,
  "pontosUsados": 0
}
```

### Obter Cartaz (Desafio 1)

```http
GET /api/Relatorio/cartaz?inicio=2026-02-10&fim=2026-02-17&disponiveis=true
```

### Obter Ocupação por Sala (Desafio 2)

```http
GET /api/Relatorio/salas/ocupacao?inicio=2026-02-01&fim=2026-02-28
```

---

## 📚 Enumerações Principais

- **ClassificacaoIndicativa**: Livre, Dez, Doze, Quatorze, Dezesseis, Dezoito
- **TipoSala**: Normal, XD, VIP, QuatroD
- **TipoSessao**: Regular, Matine, PreEstreia, Evento, EspecialBebe, EspecialPet
- **IdiomaSessao**: Dublado, Legendado, Nacional
- **FormaPagamento**: Dinheiro, Debito, Credito, PIX
- **CargoFuncionario**: Gerente, Garcom, Faxineiro, Bilheteiro
- **CategoriaProduto**: Pipoca, Bebida, Combo, Doce

---

## 👨‍💻 Autores

Vinicius Leon Paula 
Jordan Verissimo
Projeto Final de Desenvolvimento Web com .NET

---

## 📄 Licença

Este projeto foi desenvolvido como trabalho acadêmico para a disciplina de Desenvolvimento Web com .NET.

---