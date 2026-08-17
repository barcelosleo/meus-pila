using MeusPila.Application.Services;
using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.User.CreateUser;
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
        var repository = new InMemoryUserRepository([existingUser]);
        var useCase = new CreateUserUseCase(repository);

        var input = new CreateUserInputDto
        {
            Name = "New User",
            Email = "existing@mail.com",
            Password = "Valid@123",
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.AddCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenCreatedByUserDoesNotExist()
    {
        var repository = new InMemoryUserRepository();
        var useCase = new CreateUserUseCase(repository);

        var input = new CreateUserInputDto
        {
            Name = "New User",
            Email = "new.user@mail.com",
            Password = "Valid@123",
            CreatedById = Guid.NewGuid().ToString(),
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
        Assert.Equal(0, repository.AddCalls);
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
        var repository = new InMemoryUserRepository([creator]);
        var useCase = new CreateUserUseCase(repository);

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

        var createdUser = await repository.GetByIdAsync(createdId);
        Assert.NotNull(createdUser);
        Assert.Equal("john.doe@example.com", createdUser.Email);
        Assert.Equal(creator.Id, createdUser.CreatedById);
        Assert.NotEqual(input.Password, createdUser.PasswordHash);

        var passwordService = new PasswordService();
        Assert.True(passwordService.VerifyPassword(createdUser, input.Password));
        Assert.Equal(1, repository.AddCalls);
    }
}
