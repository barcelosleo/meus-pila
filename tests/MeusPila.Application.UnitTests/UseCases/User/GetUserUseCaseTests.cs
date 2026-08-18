using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.User.GetUser;
using Moq;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class GetUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenRequesterIsNotFound()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UserEntity?)null);
        var useCase = new GetUserUseCase(repository.Object);

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
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(requester.Id)).ReturnsAsync(requester);
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        var useCase = new GetUserUseCase(repository.Object);

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
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(requester.Id)).ReturnsAsync(requester);
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        repository
            .Setup(repository => repository.UserCanViewHouseholdMembersAsync(requester.Id, user.Id))
            .ReturnsAsync(false);
        var useCase = new GetUserUseCase(repository.Object);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.ExecuteAsync(new GetUserInputDto { Id = user.Id.ToString(), RequesterId = requester.Id.ToString() })
        );
    }

    private static UserEntity CreateUser(string name, string role) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = $"{name.ToLower()}@mail.com",
            PasswordHash = string.Empty,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
}
