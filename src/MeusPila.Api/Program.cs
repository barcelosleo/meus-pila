using MeusPila.Api;
using MeusPila.Application;
using MeusPila.Infrastructure;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

string corsPolicy = app.Configuration["AppSettings:CorsPolicy"] ?? "AllowFrontend";

app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseAuthorization();
app.MapControllers().RequireCors(corsPolicy);

app.UseExceptionHandler();

app.Run();

// Exposed so MeusPila.IntegrationTests can host the API via WebApplicationFactory<Program>.
public partial class Program { }
