using MeusPila.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace MeusPila.IntegrationTests.Fixtures;

/// <summary>
/// Spins up a real Postgres container (matching the image used in docker/db/Dockerfile) so
/// repository-backed tests exercise the actual EF Core mappings instead of an in-memory fake.
/// </summary>
public sealed class DatabaseFixture : IAsyncLifetime
{
    private static readonly string[] TablesToReset =
    [
        "\"FinancialAccountTransactions\"",
        "\"FinancialTransactions\"",
        "\"GroupUsers\"",
        "\"Groups\"",
        "\"FinancialAccount\"",
        "\"FinancialCategories\"",
        "\"Banks\"",
        "\"Users\"",
    ];

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18.6-alpine")
        .WithDatabase("meuspila_test")
        .WithUsername("meuspila_test")
        .WithPassword("meuspila_test")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using ApplicationDbContext context = CreateDbContext();
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public string ConnectionString => _container.GetConnectionString();

    public ApplicationDbContext CreateDbContext()
    {
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;
        return new ApplicationDbContext(options);
    }

    public async Task ResetAsync()
    {
        await using ApplicationDbContext context = CreateDbContext();
        string sql = string.Concat("TRUNCATE TABLE ", string.Join(", ", TablesToReset), " RESTART IDENTITY CASCADE");
        await context.Database.ExecuteSqlRawAsync(sql);
    }
}
