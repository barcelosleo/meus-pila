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
        var existingUser = new UserEntity("Existing", "existing@mail.com", "hashed");
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
    public async Task ExecuteAsync_ShouldCreateUserWithHashedPasswordAndLowerCaseEmail()
    {
        var repository = new InMemoryUserRepository();
        var useCase = new CreateUserUseCase(repository);

        var input = new CreateUserInputDto
        {
            Name = "John",
            Email = "John.DOE@Example.COM",
            Password = "Valid@123",
        };

        var output = await useCase.ExecuteAsync(input);

        Assert.Equal("John", output.Name);
        Assert.Equal("john.doe@example.com", output.Email);
        Assert.True(Guid.TryParse(output.Id, out var createdId));

        var createdUser = await repository.GetByIdAsync(createdId);
        Assert.NotNull(createdUser);
        Assert.Equal("john.doe@example.com", createdUser.Email);
        Assert.NotEqual(input.Password, createdUser.PasswordHash);

        var passwordService = new PasswordService();
        Assert.True(passwordService.VerifyPassword(createdUser, input.Password));
        Assert.Equal(1, repository.AddCalls);
    }
}
