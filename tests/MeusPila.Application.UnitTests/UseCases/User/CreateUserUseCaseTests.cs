using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using MeusPila.Application.UseCases.User.CreateUser;
using Moq;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class CreateUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        var existingUser = new UserEntity
        {
            Name = "Existing",
            Email = "existing@mail.com",
            PasswordHash = "hashed",
            IsActive = true,
            Role = "User",
        };
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(repository =>
                repository.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(1);
        var useCase = new CreateUserUseCase(repository.Object);

        var input = new CreateUserInputDto
        {
            Name = "New User",
            Email = "existing@mail.com",
            Password = "Valid@123",
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.AddAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenCreatedByUserDoesNotExist()
    {
        var repository = new Mock<IUserRepository>();
        repository
            .SetupSequence(repository =>
                repository.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(0)
            .ReturnsAsync(0);
        var useCase = new CreateUserUseCase(repository.Object);

        var input = new CreateUserInputDto
        {
            Name = "New User",
            Email = "new.user@mail.com",
            Password = "Valid@123",
            CreatedById = Guid.NewGuid().ToString(),
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        repository.Verify(repository => repository.AddAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateUserWithHashedPasswordAndLowerCaseEmail()
    {
        var creator = new UserEntity
        {
            Name = "Admin",
            Email = "admin@example.com",
            PasswordHash = "hashed",
            IsActive = true,
            Role = "User",
        };
        UserEntity? createdUser = null;
        var repository = new Mock<IUserRepository>();
        repository
            .SetupSequence(repository =>
                repository.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(0)
            .ReturnsAsync(1);
        repository
            .Setup(repository => repository.AddAsync(It.IsAny<UserEntity>()))
            .Callback<UserEntity>(user => createdUser = user)
            .ReturnsAsync(
                (UserEntity user) =>
                {
                    user.Id = Guid.NewGuid();
                    return user;
                }
            );
        var useCase = new CreateUserUseCase(repository.Object);

        var input = new CreateUserInputDto
        {
            Name = "John",
            Email = "John.DOE@Example.COM",
            Password = "Valid@123",
            CreatedById = creator.Id.ToString(),
        };

        var output = await useCase.ExecuteAsync(input);

        Assert.Equal("John", output.Name);
        Assert.Equal("john.doe@example.com", output.Email);
        Assert.True(Guid.TryParse(output.Id, out var createdId));

        var persistedUser = Assert.IsType<UserEntity>(createdUser);
        Assert.Equal("john.doe@example.com", persistedUser.Email);
        Assert.Equal(creator.Id, persistedUser.CreatedById);
        Assert.NotEqual(input.Password, persistedUser.PasswordHash);

        var passwordService = new PasswordService();
        Assert.True(passwordService.VerifyPassword(persistedUser, input.Password));
        repository.Verify(repository => repository.AddAsync(persistedUser), Times.Once);
    }
}
