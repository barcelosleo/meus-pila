using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.User.AuthenticateUser;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class AuthenticateUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new InMemoryUserRepository();
        var passwordService = new FakePasswordService { VerifyResult = true };
        var jwtService = new FakeJwtService();
        var useCase = new AuthenticateUserUseCase(repository, passwordService, jwtService);

        var input = new AuthenticateUserInputDto { Email = "missing@mail.com", Password = "secret" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPasswordIsInvalid()
    {
        var user = new UserEntity
        {
            Name = "Jane",
            Email = "jane@mail.com",
            PasswordHash = "hash",
            IsActive = false,
            Role = "User",
        };
        var repository = new InMemoryUserRepository([user]);
        var passwordService = new FakePasswordService { VerifyResult = false };
        var jwtService = new FakeJwtService();
        var useCase = new AuthenticateUserUseCase(repository, passwordService, jwtService);

        var input = new AuthenticateUserInputDto { Email = "jane@mail.com", Password = "wrong" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserIsInactive()
    {
        var user = new UserEntity
        {
            Name = "Jane",
            Email = "jane@mail.com",
            PasswordHash = "Hash#123asdasd",
            IsActive = false,
            Role = "User",
        };
        var repository = new InMemoryUserRepository([user]);
        var passwordService = new FakePasswordService { VerifyResult = true };
        var jwtService = new FakeJwtService();
        var useCase = new AuthenticateUserUseCase(repository, passwordService, jwtService);

        var input = new AuthenticateUserInputDto { Email = "jane@mail.com", Password = "secret" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnTokensAndPersistRefreshToken_WhenCredentialsAreValid()
    {
        var user = new UserEntity
        {
            Name = "Jane",
            Email = "jane@mail.com",
            PasswordHash = "Hash#123asdasd",
            IsActive = true,
            Role = "User",
        };
        var repository = new InMemoryUserRepository([user]);
        var passwordService = new FakePasswordService { VerifyResult = true };
        var jwtService = new FakeJwtService
        {
            AccessTokenToReturn = "access-123",
            RefreshTokenToReturn = "refresh-123",
        };

        var useCase = new AuthenticateUserUseCase(repository, passwordService, jwtService);
        var input = new AuthenticateUserInputDto { Email = "jane@mail.com", Password = "secret" };

        var output = await useCase.ExecuteAsync(input);

        Assert.Equal(user.Id.ToString(), output.Id);
        Assert.Equal("Jane", output.Name);
        Assert.Equal("jane@mail.com", output.Email);
        Assert.Equal("access-123", output.AccessToken);
        Assert.Equal("refresh-123", output.RefreshToken);
        Assert.Equal(1, repository.UpdateCalls);

        var updatedUser = await repository.GetByIdAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("refresh-123", updatedUser!.RefreshToken);
        Assert.True(updatedUser.RefreshTokenExpiryTime > DateTime.UtcNow.AddDays(6));
    }
}
