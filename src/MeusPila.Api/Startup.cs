using System.Text.Json.Serialization;
using MeusPila.Infrastructure;

namespace MeusPila.Api;

public static class Startup
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddOpenApi();
        services.AddHttpContextAccessor();
        services.AddCors(configuration);
        return services;
    }

    private static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        string corsPolicy = configuration["AppSettings:CorsPolicy"] ?? "AllowFrontend";
        string[] withOrigins =
            configuration.GetSection("AppSettings:WithOrigins").Get<string[]>() ?? ["http://localhost:5173"];

        services.AddCors(options =>
            options.AddPolicy(
                name: corsPolicy,
                policy => policy.WithOrigins(withOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()
            )
        );
        return services;
    }
}
