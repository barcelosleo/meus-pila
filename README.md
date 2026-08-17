# Meus Pila
Aplicacao para o gerenciamento de financas

## Project structure
```text
📁 YourSolution/
│
├── 📁 src/                                # Contains all production source code projects
│   │
│   ├── 📁 MeusPila.Domain/                # [Class Library] Inner core: Entities & Business Rules
│   │   ├── 📁 Aggregates/                 # Optional: Higher-level root boundaries
│   │   ├── 📁 Common/                     # Base abstract entity classes, value object definitions
│   │   ├── 📁 Entities/                   # Database-agnostic business objects (e.g., Order.cs)
│   │   ├── 📁 Enums/                      # Domain-specific enumerations
│   │   ├── 📁 Events/                     # Domain Events (side effects/notifications)
│   │   ├── 📁 Exceptions/                 # Custom domain-level exceptions
│   │   ├── 📁 Interfaces/                 # Repository contracts and abstractions
│   │   ├── 📁 Services/                   # Business logic spanning multiple entities
│   │   └── 📁 ValueObjects/               # Immutable objects without identities (e.g., Address.cs)
│   │
│   ├── 📁 MeusPila.Application/           # [Class Library] Orchestration: Use Cases & Interfaces
│   │   ├── 📁 Behaviours/                 # Logging, validation, and caching pipeline behaviors
│   │   ├── 📁 Interfaces/                 # Repository contracts (e.g., IOrderRepository.cs)
│   │   └── 📁 UseCases/                   # Use cases implementations
│   │
│   ├── 📁 MeusPila.Infrastructure/        # [Class Library] Adapters: Databases, APIs, Files
│   │   ├── 📁 Authentication/             # JWT generation, token validation implementations
│   │   ├── 📁 Data/                       # EF Core DbContext, Migrations, configurations
│   │   ├── 📁 Repositories/               # Concrete database implementations of Domain interfaces
│   │   └── 📁 Services/                   # Third-party integrations (e.g., SendGridEmailService.cs)
│   │
│   └── 📁 MeusPila.Api/                   # [Web API / UI] Entry Point: Endpoints & Routing
│       ├── 📁 Controllers/                # REST endpoints or Minimal API endpoint mappings
│       ├── 📁 Middlewares/                # Global error handling, custom request interception
│       ├── 📁 Extensions/                 # Dependency injection setup (WebApplication extensions)
│       └── 📄 Program.cs                  # Composition root to wire up all layers
│
└── 📁 tests/                               # Parallel directory containing test suites
    ├── 📁 MeusPila.Domain.UnitTests/       # Business logic validation tests
    ├── 📁 MeusPila.Application.UnitTests/  # Use case behavior validation tests
    └── 📁 MeusPila.IntegrationTests/       # API endpoint and database integration tests

```

## ConsoleApp

O projeto inclui um console app para executar comandos de manutencao de usuarios usando a mesma configuracao de banco da API.

### Exibir ajuda

```bash
dotnet run --project src/MeusPila.ConsoleApp/MeusPila.ConsoleApp.csproj -- help
```

### Criar usuario

```bash
dotnet run --project src/MeusPila.ConsoleApp/MeusPila.ConsoleApp.csproj -- create-user \
  --name "Alice" \
  --email "alice@example.com" \
  --password "Strong@123" \
  --role "Admin"
```

Opcionalmente, voce pode informar quem criou o usuario:

```bash
dotnet run --project src/MeusPila.ConsoleApp/MeusPila.ConsoleApp.csproj -- create-user \
  --name "Bob" \
  --email "bob@example.com" \
  --password "Strong@123" \
  --role "User" \
  --created-by-id "00000000-0000-0000-0000-000000000001"
```

### Atualizar usuario

```bash
dotnet run --project src/MeusPila.ConsoleApp/MeusPila.ConsoleApp.csproj -- update-user \
  --id "00000000-0000-0000-0000-000000000001" \
  --name "Alice Silva" \
  --email "alice.silva@example.com" \
  --is-active true
```

Para trocar a senha, informe a senha atual e a nova senha:

```bash
dotnet run --project src/MeusPila.ConsoleApp/MeusPila.ConsoleApp.csproj -- update-user \
  --id "00000000-0000-0000-0000-000000000001" \
  --current-password "Strong@123" \
  --new-password "NewStrong@123"
```

### Observacoes

- Os comandos usam a conexao configurada em `src/MeusPila.Api/appsettings.Development.json`.
- Antes de executar os comandos, garanta que o banco esteja disponivel.
- Para atualizar o responsavel pela alteracao, use `--updated-by-id <user-id>`.

## EF Core Migrations

Este projeto usa Entity Framework Core com PostgreSQL. A string de conexao da aplicacao esta em `src/MeusPila.Api/appsettings.Development.json` e o contexto principal e `ApplicationDbContext` em `src/MeusPila.Infrastructure/Data/ApplicationDbContext.cs`.

### Instalar o EF Core CLI

```bash
dotnet tool install --global dotnet-ef
```

Se o comando `dotnet ef` ainda nao estiver no PATH, adicione o caminho do dotnet tools ao shell atual e ao perfil:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

No shell `fish`:

```fish
fish_add_path $HOME/.dotnet/tools
```

### Iniciar o banco localmente

```bash
docker compose up -d db
```

### Gerar uma migration

```bash
dotnet ef migrations add InitialCreate \
  --project src/MeusPila.Infrastructure/MeusPila.Infrastructure.csproj \
  --startup-project src/MeusPila.Api/MeusPila.Api.csproj \
  --context ApplicationDbContext \
  --output-dir Data/Migrations
```

> Troque `InitialCreate` pelo nome da migration que quiser aplicar.

### Aplicar as migrations no banco

```bash
dotnet ef database update \
  --project src/MeusPila.Infrastructure/MeusPila.Infrastructure.csproj \
  --startup-project src/MeusPila.Api/MeusPila.Api.csproj \
  --context ApplicationDbContext
```

### Listar migrations existentes

```bash
dotnet ef migrations list \
  --project src/MeusPila.Infrastructure/MeusPila.Infrastructure.csproj \
  --startup-project src/MeusPila.Api/MeusPila.Api.csproj \
  --context ApplicationDbContext
```

### Gerar script SQL das migrations

```bash
dotnet ef migrations script \
  --project src/MeusPila.Infrastructure/MeusPila.Infrastructure.csproj \
  --startup-project src/MeusPila.Api/MeusPila.Api.csproj \
  --context ApplicationDbContext
```

### Remover a ultima migration

```bash
dotnet ef migrations remove \
  --project src/MeusPila.Infrastructure/MeusPila.Infrastructure.csproj \
  --startup-project src/MeusPila.Api/MeusPila.Api.csproj \
  --context ApplicationDbContext
```

### Remover o banco de dados

```bash
dotnet ef database drop \
  --project src/MeusPila.Infrastructure/MeusPila.Infrastructure.csproj \
  --startup-project src/MeusPila.Api/MeusPila.Api.csproj \
  --context ApplicationDbContext \
  --force
```

### Dicas

- Se o comando `dotnet ef` nao estiver disponivel, rode: `dotnet tool restore`
- Sempre rode os comandos na raiz do projeto.
- Caso haja problema com design-time, confirme que o projeto de startup e a infraestrutura estao restaurados com `dotnet restore`.

## Gerador de Use Case (VS Code)

O projeto pode usar a extensao externa para gerar automaticamente use cases por clique direito no Explorer.

### O que é gerado

Dentro da pasta indicada, sera criada uma subpasta com o nome do use case e os arquivos:

- `{USE_CASE_NAME}InputDto.cs`
- `{USE_CASE_NAME}OutputDto.cs`
- `{USE_CASE_NAME}UseCase.cs`

### Templates customizaveis

Voce pode manter templates no projeto e referenciar esses arquivos nas configuracoes da extensao:

- `useCaseScaffold.inputTemplateRelativePath`
- `useCaseScaffold.outputTemplateRelativePath`
- `useCaseScaffold.useCaseTemplateRelativePath`

Placeholders suportados:

- `{{NAMESPACE}}`
- `{{USE_CASE_NAME}}`
- `{{INPUT_DTO}}`
- `{{OUTPUT_DTO}}`

Exemplo: ao gerar `CreateInvoice` em `User`, o namespace ficara `MeusPila.Application.UseCases.User.CreateInvoice`.

## Clique Direito em Pasta (Explorer)

O clique direito foi movido para uma extensao separada, fora deste repositorio.

### Onde esta a extensao

- `/home/leonardo/Development/use-case-vscode-extension`

### Como carregar no VS Code

1. Abra o Command Palette (`Ctrl+Shift+P`)
2. Rode `Developer: Install Extension from Location...`
3. Selecione a pasta `/home/leonardo/Development/use-case-vscode-extension`
4. Se necessario, recarregue a janela (`Developer: Reload Window`)

### Como usar no clique direito

1. No Explorer, clique com o botao direito em uma pasta dentro de `src/MeusPila.Application/UseCases` (ex.: `User`)
2. Escolha `Scaffold Use Case Here`
3. Informe apenas o nome do use case (ex.: `RefreshToken`)

A extensao gera os 3 arquivos diretamente (InputDto, OutputDto e UseCase) na pasta selecionada.

### Configuracao da extensao

Na extensao externa, voce pode configurar:

- `useCaseScaffold.scriptRelativePath`
- `useCaseScaffold.targetRootRelativePath`
- `useCaseScaffold.commandName`