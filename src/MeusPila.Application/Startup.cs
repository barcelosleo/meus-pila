using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using MeusPila.Application.UseCases.Houshold.AddMember;
using MeusPila.Application.UseCases.Houshold.CreateHousehold;
using MeusPila.Application.UseCases.Houshold.GetHousehold;
using MeusPila.Application.UseCases.Houshold.ListHouseholds;
using MeusPila.Application.UseCases.Houshold.RemoveMember;
using MeusPila.Application.UseCases.Houshold.UpdateHousehold;
using MeusPila.Application.UseCases.User.AuthenticateUser;
using MeusPila.Application.UseCases.User.CreateUser;
using MeusPila.Application.UseCases.User.GetUser;
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

        // User Use Cases
        services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IListUsersUseCase, ListUsersUseCase>();
        services.AddScoped<IGetUserUseCase, GetUserUseCase>();

        // Household Use Cases
        services.AddScoped<ICreateHouseholdUseCase, CreateHouseholdUseCase>();
        services.AddScoped<IUpdateHouseholdUseCase, UpdateHouseholdUseCase>();
        services.AddScoped<IListHouseholdsUseCase, ListHouseholdsUseCase>();
        services.AddScoped<IGetHouseholdUseCase, GetHouseholdUseCase>();
        services.AddScoped<IAddMemberUseCase, AddMemberUseCase>();
        services.AddScoped<IRemoveMemberUseCase, RemoveMemberUseCase>();

        return services;
    }
}
