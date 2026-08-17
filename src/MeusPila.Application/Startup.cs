using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using MeusPila.Application.UseCases.User.AuthenticateUser;
using MeusPila.Application.UseCases.User.CreateUser;
using MeusPila.Application.UseCases.User.ListUsers;
using MeusPila.Application.UseCases.User.RefreshToken;
using MeusPila.Application.UseCases.User.UpdateUser;
using Microsoft.Extensions.DependencyInjection;

namespace MeusPila.Application;

public static class Startup
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IListUsersUseCase, ListUsersUseCase>();

        return services;
    }
}
