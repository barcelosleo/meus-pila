using System.Text;
using MeusPila.Application.Interfaces;
using MeusPila.Infrastructure.Data;
using MeusPila.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MeusPila.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();
        services.AddDbContext(configuration);
        services.AddAuthentication(configuration);
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("GroupManagerOnly", policy => policy.RequireRole("GroupManager"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
        });
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection string is not configured.");
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        string issuer =
            configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
        string audience =
            configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
        string secretKey =
            configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret key is not configured.");
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuerSigningKey = true,
                }
            );
        return services;
    }
}
