using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.User.ListUsers;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class ListUsersUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPageSizeIsInvalid()
    {
        var useCase = new ListUsersUseCase(new InMemoryUserRepository());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(new ListUsersInputDto { PageNumber = 1, PageSize = 0 })
        );
    }

    [Fact]
    public async Task ExecuteAsync_ShouldFilterAndPaginateUsers()
    {
        var users = new[]
        {
            CreateUser("Alice", "alice@mail.com"),
            CreateUser("Bob", "bob@mail.com"),
            CreateUser("Alicia", "alicia@mail.com"),
        };
        var useCase = new ListUsersUseCase(new InMemoryUserRepository(users));

        var output = await useCase.ExecuteAsync(
            new ListUsersInputDto
            {
                PageNumber = 1,
                PageSize = 1,
                SearchTerm = " ali ",
            }
        );

        Assert.Equal(2, output.TotalCount);
        Assert.Equal(2, output.TotalPages);
        Assert.Single(output.Items);
        Assert.Equal("Alice", output.Items.Single().Name);
    }

    private static UserEntity CreateUser(string name, string email) =>
        new()
        {
            Name = name,
            Email = email,
            PasswordHash = string.Empty,
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
}
