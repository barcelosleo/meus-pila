using MeusPila.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MeusPila.IntegrationTests.Fixtures;

/// <summary>
/// Hosts the real MeusPila.Api application, pointed at the DatabaseFixture's Testcontainers Postgres instance.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Program.cs reads the connection string eagerly when registering the DbContext, so appsettings-level
        // overrides arrive too late; replace the DbContext registration itself instead.
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_connectionString));
        });
    }
}
