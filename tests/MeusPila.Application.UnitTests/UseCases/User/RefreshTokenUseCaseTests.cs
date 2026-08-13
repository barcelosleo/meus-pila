using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.User.RefreshToken;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class RefreshTokenUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new InMemoryUserRepository();
        var jwtService = new FakeJwtService();
        var useCase = new RefreshTokenUseCase(repository, jwtService);

        var input = new RefreshTokenInputDto { UserId = Guid.NewGuid(), RefreshToken = "token" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenTokenDoesNotMatch()
    {
        var user = new UserEntity("Ana", "ana@mail.com", "hash")
        {
            RefreshToken = "right-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1),
        };

        var repository = new InMemoryUserRepository([user]);
        var jwtService = new FakeJwtService();
        var useCase = new RefreshTokenUseCase(repository, jwtService);

        var input = new RefreshTokenInputDto { UserId = user.Id, RefreshToken = "wrong-token" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenTokenIsExpired()
    {
        var user = new UserEntity("Ana", "ana@mail.com", "hash")
        {
            RefreshToken = "token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1),
        };

        var repository = new InMemoryUserRepository([user]);
        var jwtService = new FakeJwtService();
        var useCase = new RefreshTokenUseCase(repository, jwtService);

        var input = new RefreshTokenInputDto { UserId = user.Id, RefreshToken = "token" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNewTokensAndPersistThem_WhenRefreshIsValid()
    {
        var user = new UserEntity("Ana", "ana@mail.com", "hash")
        {
            RefreshToken = "old-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1),
        };

        var repository = new InMemoryUserRepository([user]);
        var jwtService = new FakeJwtService
        {
            AccessTokenToReturn = "new-access",
            RefreshTokenToReturn = "new-refresh",
        };

        var useCase = new RefreshTokenUseCase(repository, jwtService);
        var input = new RefreshTokenInputDto { UserId = user.Id, RefreshToken = "old-token" };

        var output = await useCase.ExecuteAsync(input);

        Assert.Equal("new-access", output.AccessToken);
        Assert.Equal("new-refresh", output.RefreshToken);
        Assert.Equal(1, repository.UpdateCalls);

        var updatedUser = await repository.GetByIdAsync(user.Id);
        Assert.Equal("new-refresh", updatedUser.RefreshToken);
        Assert.True(updatedUser.RefreshTokenExpiryTime > DateTime.UtcNow.AddDays(6));
    }
}
