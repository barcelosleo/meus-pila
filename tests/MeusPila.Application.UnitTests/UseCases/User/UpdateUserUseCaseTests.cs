using MeusPila.Application.Services;
using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.User.UpdateUser;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class UpdateUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new InMemoryUserRepository();
        var useCase = new UpdateUserUseCase(repository);

        var input = new UpdateUserInputDto { Id = Guid.NewGuid().ToString(), Name = "Any" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenEmailIsAlreadyInUseByAnotherUser()
    {
        var currentUser = CreateUserWithPassword("Current", "current@mail.com", "Current@123");
        var otherUser = CreateUserWithPassword("Other", "used@mail.com", "Other@123");
        var repository = new InMemoryUserRepository([currentUser, otherUser]);
        var useCase = new UpdateUserUseCase(repository);

        var input = new UpdateUserInputDto { Id = currentUser.Id.ToString(), Email = "used@mail.com" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenChangingPasswordWithoutCurrentPassword()
    {
        var user = CreateUserWithPassword("User", "user@mail.com", "Current@123");
        var repository = new InMemoryUserRepository([user]);
        var useCase = new UpdateUserUseCase(repository);

        var input = new UpdateUserInputDto { Id = user.Id.ToString(), NewPassword = "NewPass@123" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenCurrentPasswordIsIncorrect()
    {
        var user = CreateUserWithPassword("User", "user@mail.com", "Current@123");
        var repository = new InMemoryUserRepository([user]);
        var useCase = new UpdateUserUseCase(repository);

        var input = new UpdateUserInputDto
        {
            Id = user.Id.ToString(),
            CurrentPassword = "Wrong@123",
            NewPassword = "NewPass@123",
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateUserDataAndPassword_WhenInputIsValid()
    {
        var user = CreateUserWithPassword("User", "user@mail.com", "Current@123");
        var oldHash = user.PasswordHash;
        var repository = new InMemoryUserRepository([user]);
        var useCase = new UpdateUserUseCase(repository);

        var input = new UpdateUserInputDto
        {
            Id = user.Id.ToString(),
            Name = "Updated Name",
            Email = "updated@mail.com",
            IsActive = false,
            CurrentPassword = "Current@123",
            NewPassword = "NewPass@123",
        };

        var output = await useCase.ExecuteAsync(input);

        Assert.Equal(user.Id.ToString(), output.Id);
        Assert.Equal("Updated Name", output.Name);
        Assert.Equal("updated@mail.com", output.Email);
        Assert.False(output.IsActive);
        Assert.Equal(1, repository.UpdateCalls);

        var updatedUser = await repository.GetByIdAsync(user.Id);
        Assert.Equal("Updated Name", updatedUser.Name);
        Assert.Equal("updated@mail.com", updatedUser.Email);
        Assert.False(updatedUser.IsActive);
        Assert.NotEqual(oldHash, updatedUser.PasswordHash);

        var passwordService = new PasswordService();
        Assert.True(passwordService.VerifyPassword(updatedUser, "NewPass@123"));
    }

    private static UserEntity CreateUserWithPassword(string name, string email, string password)
    {
        var user = new UserEntity(name, email, password);
        var passwordService = new PasswordService();
        user.PasswordHash = passwordService.HashPassword(user, password);
        return user;
    }
}
