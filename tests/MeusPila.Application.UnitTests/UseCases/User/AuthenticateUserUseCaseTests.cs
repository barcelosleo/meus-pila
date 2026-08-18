using MeusPila.Application.Interfaces;
using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.User.AuthenticateUser;
using Moq;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class AuthenticateUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(repository =>
                repository.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync((UserEntity?)null);
        var passwordService = new FakePasswordService { VerifyResult = true };
        var jwtService = new FakeJwtService();
        var useCase = new AuthenticateUserUseCase(repository.Object, passwordService, jwtService);

        var input = new AuthenticateUserInputDto { Email = "missing@mail.com", Password = "secret" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
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
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(repository =>
                repository.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(user);
        var passwordService = new FakePasswordService { VerifyResult = false };
        var jwtService = new FakeJwtService();
        var useCase = new AuthenticateUserUseCase(repository.Object, passwordService, jwtService);

        var input = new AuthenticateUserInputDto { Email = "jane@mail.com", Password = "wrong" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
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
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(repository =>
                repository.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(user);
        var passwordService = new FakePasswordService { VerifyResult = true };
        var jwtService = new FakeJwtService();
        var useCase = new AuthenticateUserUseCase(repository.Object, passwordService, jwtService);

        var input = new AuthenticateUserInputDto { Email = "jane@mail.com", Password = "secret" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
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
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(repository =>
                repository.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(user);
        repository.Setup(repository => repository.UpdateAsync(user)).ReturnsAsync(user);
        var passwordService = new FakePasswordService { VerifyResult = true };
        var jwtService = new FakeJwtService
        {
            AccessTokenToReturn = "access-123",
            RefreshTokenToReturn = "refresh-123",
        };

        var useCase = new AuthenticateUserUseCase(repository.Object, passwordService, jwtService);
        var input = new AuthenticateUserInputDto { Email = "jane@mail.com", Password = "secret" };

        var output = await useCase.ExecuteAsync(input);

        Assert.Equal(user.Id.ToString(), output.Id);
        Assert.Equal("Jane", output.Name);
        Assert.Equal("jane@mail.com", output.Email);
        Assert.Equal("access-123", output.AccessToken);
        Assert.Equal("refresh-123", output.RefreshToken);
        repository.Verify(repository => repository.UpdateAsync(user), Times.Once);
        Assert.Equal("refresh-123", user.RefreshToken);
        Assert.True(user.RefreshTokenExpiryTime > DateTime.UtcNow.AddDays(6));
    }
}
