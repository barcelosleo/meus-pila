using MeusPila.Application.Interfaces;
using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.User.RefreshToken;
using Moq;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class RefreshTokenUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UserEntity?)null);
        var jwtService = new FakeJwtService();
        var useCase = new RefreshTokenUseCase(repository.Object, jwtService);

        var input = new RefreshTokenInputDto { UserId = Guid.NewGuid(), RefreshToken = "token" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenTokenDoesNotMatch()
    {
        var user = new UserEntity
        {
            Name = "Ana",
            Email = "ana@mail.com",
            PasswordHash = "hash",
            IsActive = true,
            Role = "User",
            RefreshToken = "right-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1),
        };

        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        var jwtService = new FakeJwtService();
        var useCase = new RefreshTokenUseCase(repository.Object, jwtService);

        var input = new RefreshTokenInputDto { UserId = user.Id, RefreshToken = "wrong-token" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenTokenIsExpired()
    {
        var user = new UserEntity
        {
            Name = "Ana",
            Email = "ana@mail.com",
            PasswordHash = "hash",
            IsActive = true,
            Role = "User",
            RefreshToken = "token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1),
        };

        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        var jwtService = new FakeJwtService();
        var useCase = new RefreshTokenUseCase(repository.Object, jwtService);

        var input = new RefreshTokenInputDto { UserId = user.Id, RefreshToken = "token" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNewTokensAndPersistThem_WhenRefreshIsValid()
    {
        var user = new UserEntity
        {
            Name = "Ana",
            Email = "ana@mail.com",
            PasswordHash = "hash",
            IsActive = true,
            Role = "User",
            RefreshToken = "old-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1),
        };

        var repository = new Mock<IUserRepository>();
        repository.Setup(repository => repository.GetByIdAsync(user.Id)).ReturnsAsync(user);
        repository.Setup(repository => repository.UpdateAsync(user)).ReturnsAsync(user);
        var jwtService = new FakeJwtService
        {
            AccessTokenToReturn = "new-access",
            RefreshTokenToReturn = "new-refresh",
        };

        var useCase = new RefreshTokenUseCase(repository.Object, jwtService);
        var input = new RefreshTokenInputDto { UserId = user.Id, RefreshToken = "old-token" };

        var output = await useCase.ExecuteAsync(input);

        Assert.Equal("new-access", output.AccessToken);
        Assert.Equal("new-refresh", output.RefreshToken);
        repository.Verify(repository => repository.UpdateAsync(user), Times.Once);
        Assert.Equal("new-refresh", user.RefreshToken);
        Assert.True(user.RefreshTokenExpiryTime > DateTime.UtcNow.AddDays(6));
    }
}
