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