using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.User.GetUser;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class GetUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenRequesterIsNotFound()
    {
        var repository = new InMemoryUserRepository();
        var useCase = new GetUserUseCase(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new GetUserInputDto { Id = Guid.NewGuid().ToString(), RequesterId = Guid.NewGuid().ToString() }
            )
        );
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnUser_WhenAdminRequestsUser()
    {
        var requester = CreateUser("Admin", "Admin");
        var user = CreateUser("User", "User");
        var repository = new InMemoryUserRepository([requester, user]);
        var useCase = new GetUserUseCase(repository);

        var output = await useCase.ExecuteAsync(
            new GetUserInputDto { Id = user.Id.ToString(), RequesterId = requester.Id.ToString() }
        );

        Assert.Equal(user.Id.ToString(), output.Id);
        Assert.Equal(user.Name, output.Name);
        Assert.Equal(user.Email, output.Email);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenManagerCannotViewUser()
    {
        var requester = CreateUser("Manager", "HouseholdManager");
        var user = CreateUser("User", "User");
        var repository = new InMemoryUserRepository([requester, user]);
        var useCase = new GetUserUseCase(repository);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.ExecuteAsync(new GetUserInputDto { Id = user.Id.ToString(), RequesterId = requester.Id.ToString() })
        );
    }

    private static UserEntity CreateUser(string name, string role) =>
        new()
        {
            Name = name,
            Email = $"{name.ToLower()}@mail.com",
            PasswordHash = string.Empty,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
}
