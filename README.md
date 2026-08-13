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