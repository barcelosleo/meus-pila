using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using MeusPila.Application.UseCases.User.UpdateUser;
using Moq;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class UpdateUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UserEntity?)null);
        var useCase = new UpdateUserUseCase(repository.Object);

        var input = new UpdateUserInputDto { Id = Guid.NewGuid().ToString(), Name = "Any" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenEmailIsAlreadyInUseByAnotherUser()
    {
        var currentUser = CreateUserWithPassword("Current", "current@mail.com", "Current@123");
        var otherUser = CreateUserWithPassword("Other", "used@mail.com", "Other@123");
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(currentUser.Id)).ReturnsAsync(currentUser);
        repository
            .Setup(repository =>
                repository.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(1);
        var useCase = new UpdateUserUseCase(repository.Object);

        var input = new UpdateUserInputDto { Id = currentUser.Id.ToString(), Email = "used@mail.com" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenChangingPasswordWithoutCurrentPassword()
    {
        var user = CreateUserWithPassword("User", "user@mail.com", "Current@123");
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        var useCase = new UpdateUserUseCase(repository.Object);

        var input = new UpdateUserInputDto { Id = user.Id.ToString(), NewPassword = "NewPass@123" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenCurrentPasswordIsIncorrect()
    {
        var user = CreateUserWithPassword("User", "user@mail.com", "Current@123");
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        var useCase = new UpdateUserUseCase(repository.Object);

        var input = new UpdateUserInputDto
        {
            Id = user.Id.ToString(),
            CurrentPassword = "Wrong@123",
            NewPassword = "NewPass@123",
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUpdatedByUserDoesNotExist()
    {
        var user = CreateUserWithPassword("User", "user@mail.com", "Current@123");
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        repository
            .Setup(repository =>
                repository.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(0);
        var useCase = new UpdateUserUseCase(repository.Object);

        var input = new UpdateUserInputDto
        {
            Id = user.Id.ToString(),
            UpdatedById = Guid.NewGuid().ToString(),
            Name = "Updated Name",
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateUserDataAndPassword_WhenInputIsValid()
    {
        var user = CreateUserWithPassword("User", "user@mail.com", "Current@123");
        var updater = CreateUserWithPassword("Updater", "updater@mail.com", "Updater@123");
        var oldHash = user.PasswordHash;
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        repository
            .SetupSequence(repository =>
                repository.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(0)
            .ReturnsAsync(1);
        repository.Setup(repository => repository.UpdateAsync(user)).ReturnsAsync(user);
        var useCase = new UpdateUserUseCase(repository.Object);

        var input = new UpdateUserInputDto
        {
            Id = user.Id.ToString(),
            Name = "Updated Name",
            Email = "updated@mail.com",
            IsActive = false,
            CurrentPassword = "Current@123",
            NewPassword = "NewPass@123",
            UpdatedById = updater.Id.ToString(),
        };

        var output = await useCase.ExecuteAsync(input);

        Assert.Equal(user.Id.ToString(), output.Id);
        Assert.Equal("Updated Name", output.Name);
        Assert.Equal("updated@mail.com", output.Email);
        Assert.False(output.IsActive);
        repository.Verify(repository => repository.UpdateAsync(user), Times.Once);
        Assert.Equal("Updated Name", user.Name);
        Assert.Equal("updated@mail.com", user.Email);
        Assert.False(user.IsActive);
        Assert.Equal(updater.Id, user.UpdatedById);
        Assert.NotEqual(oldHash, user.PasswordHash);

        var passwordService = new PasswordService();
        Assert.True(passwordService.VerifyPassword(user, "NewPass@123"));
    }

    private static UserEntity CreateUserWithPassword(string name, string email, string password)
    {
        var user = new UserEntity
        {
            Name = name,
            Email = email,
            IsActive = true,
            Role = "User",
            PasswordHash = string.Empty, // Will be set after hashing
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var passwordService = new PasswordService();
        user.PasswordHash = passwordService.HashPassword(user, password);
        return user;
    }
}
