using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using MeusPila.Application.UseCases.Admin.CreateUser;
using MeusPila.Application.UseCases.Admin.ListUsers;
using MeusPila.Application.UseCases.Bank.CreateBank;
using MeusPila.Application.UseCases.Bank.ListBanks;
using MeusPila.Application.UseCases.Group.AddMember;
using MeusPila.Application.UseCases.Group.CreateGroup;
using MeusPila.Application.UseCases.Group.GetGroup;
using MeusPila.Application.UseCases.Group.ListGroups;
using MeusPila.Application.UseCases.Group.RemoveGroup;
using MeusPila.Application.UseCases.Group.RemoveMember;
using MeusPila.Application.UseCases.Group.UpdateGroup;
using MeusPila.Application.UseCases.User.AuthenticateUser;
using MeusPila.Application.UseCases.User.GetUser;
using MeusPila.Application.UseCases.User.RefreshToken;
using MeusPila.Application.UseCases.User.RemoveUser;
using MeusPila.Application.UseCases.User.UpdateUser;
using Microsoft.Extensions.DependencyInjection;

namespace MeusPila.Application;

public static class Startup
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        // Admin Use Cases
        services.AddScoped<IListUsersUseCase, ListUsersUseCase>();
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();

        // Bank Use Cases
        services.AddScoped<IListBanksUseCase, ListBanksUseCase>();
        services.AddScoped<ICreateBankUseCase, CreateBankUseCase>();

        // User Use Cases
        services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
        services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IGetUserUseCase, GetUserUseCase>();
        services.AddScoped<IRemoveUserUseCase, RemoveUserUseCase>();

        // Group Use Cases
        services.AddScoped<ICreateGroupUseCase, CreateGroupUseCase>();
        services.AddScoped<IUpdateGroupUseCase, UpdateGroupUseCase>();
        services.AddScoped<IListGroupsUseCase, ListGroupsUseCase>();
        services.AddScoped<IGetGroupUseCase, GetGroupUseCase>();
        services.AddScoped<IAddMemberUseCase, AddMemberUseCase>();
        services.AddScoped<IRemoveMemberUseCase, RemoveMemberUseCase>();
        services.AddScoped<IRemoveGroupUseCase, RemoveGroupUseCase>();

        return services;
    }
}
