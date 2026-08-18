using System.ComponentModel.DataAnnotations;
using MeusPila.Application;
using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.User.CreateUser;
using MeusPila.Application.UseCases.User.UpdateUser;
using MeusPila.Infrastructure.Data;
using MeusPila.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

ServiceProvider serviceProvider = BuildServices();

return await ExecuteAsync(args, serviceProvider);

static async Task<int> ExecuteAsync(string[] args, ServiceProvider serviceProvider)
{
    if (args.Length == 0)
    {
        PrintUsage();
        return 1;
    }

    try
    {
        return args[0].ToLowerInvariant() switch
        {
            "create-user" => await CreateUserAsync(args[1..], serviceProvider),
            "update-user" => await UpdateUserAsync(args[1..], serviceProvider),
            "help" or "--help" or "-h" => PrintHelpAndReturn(),
            _ => UnknownCommand(args[0]),
        };
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return 1;
    }
}

static ServiceProvider BuildServices()
{
    string apiProjectPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MeusPila.Api")
    );

    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(apiProjectPath)
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile("appsettings.Development.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    string connectionString =
        configuration["ConnectionStrings:DefaultConnection"]
        ?? throw new InvalidOperationException("Database connection string is not configured.");

    var services = new ServiceCollection();
    services.AddApplicationServices();
    services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
    services.AddScoped<IUserRepository, UserRepository>();

    return services.BuildServiceProvider();
}

static async Task<int> CreateUserAsync(string[] args, ServiceProvider serviceProvider)
{
    Dictionary<string, string> options = ParseOptions(args);
    var input = new CreateUserInputDto
    {
        Name = GetRequiredOption(options, "name"),
        Email = GetRequiredOption(options, "email"),
        Password = GetRequiredOption(options, "password"),
        Role = GetRequiredOption(options, "role"),
        CreatedById = GetOptionalOption(options, "created-by-id"),
    };

    Validate(input);

    using IServiceScope scope = serviceProvider.CreateScope();
    var useCase = scope.ServiceProvider.GetRequiredService<ICreateUserUseCase>();
    CreateUserOutputDto output = await useCase.ExecuteAsync(input);

    Console.WriteLine($"User created: {output.Id}");
    Console.WriteLine($"Name: {output.Name}");
    Console.WriteLine($"Email: {output.Email}");
    return 0;
}

static async Task<int> UpdateUserAsync(string[] args, ServiceProvider serviceProvider)
{
    Dictionary<string, string> options = ParseOptions(args);
    var input = new UpdateUserInputDto
    {
        Id = GetRequiredOption(options, "id"),
        Name = GetOptionalOption(options, "name"),
        Email = GetOptionalOption(options, "email"),
        CurrentPassword = GetOptionalOption(options, "current-password"),
        NewPassword = GetOptionalOption(options, "new-password"),
        IsActive = ParseOptionalBoolean(options, "is-active"),
        UpdatedById = GetOptionalOption(options, "updated-by-id"),
    };

    Validate(input);

    using IServiceScope scope = serviceProvider.CreateScope();
    var useCase = scope.ServiceProvider.GetRequiredService<IUpdateUserUseCase>();
    UpdateUserOutputDto output = await useCase.ExecuteAsync(input);

    Console.WriteLine($"User updated: {output.Id}");
    Console.WriteLine($"Name: {output.Name}");
    Console.WriteLine($"Email: {output.Email}");
    Console.WriteLine($"IsActive: {output.IsActive}");
    return 0;
}

static Dictionary<string, string> ParseOptions(string[] args)
{
    var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    for (int index = 0; index < args.Length; index++)
    {
        string arg = args[index];
        if (!arg.StartsWith("--", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Invalid argument '{arg}'. Options must start with '--'.");
        }

        if (index == args.Length - 1)
        {
            throw new InvalidOperationException($"Missing value for option '{arg}'.");
        }

        string optionName = arg[2..];
        string optionValue = args[++index];
        options[optionName] = optionValue;
    }

    return options;
}

static string GetRequiredOption(IReadOnlyDictionary<string, string> options, string name)
{
    if (!options.TryGetValue(name, out string? value) || string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"Missing required option '--{name}'.");
    }

    return value;
}

static string? GetOptionalOption(IReadOnlyDictionary<string, string> options, string name)
{
    return options.TryGetValue(name, out string? value) && !string.IsNullOrWhiteSpace(value) ? value : null;
}

static bool? ParseOptionalBoolean(IReadOnlyDictionary<string, string> options, string name)
{
    if (!options.TryGetValue(name, out string? value) || string.IsNullOrWhiteSpace(value))
    {
        return null;
    }

    if (bool.TryParse(value, out bool parsedValue))
    {
        return parsedValue;
    }

    throw new InvalidOperationException($"Option '--{name}' expects 'true' or 'false'.");
}

static void Validate<T>(T model)
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(model!);

    if (Validator.TryValidateObject(model!, validationContext, validationResults, validateAllProperties: true))
    {
        return;
    }

    throw new InvalidOperationException(
        string.Join(Environment.NewLine, validationResults.Select(result => result.ErrorMessage))
    );
}

static int PrintHelpAndReturn()
{
    PrintUsage();
    return 0;
}

static int UnknownCommand(string command)
{
    Console.Error.WriteLine($"Unknown command '{command}'.");
    PrintUsage();
    return 1;
}

static void PrintUsage()
{
    Console.WriteLine("Available commands:");
    Console.WriteLine(
        "  create-user --name <name> --email <email> --password <password> --role <role> [--created-by-id <user-id>]"
    );
    Console.WriteLine(
        "  update-user --id <user-id> [--name <name>] [--email <email>] [--current-password <password> --new-password <password>] [--is-active <true|false>] [--updated-by-id <user-id>]"
    );
}
