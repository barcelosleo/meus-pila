using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.User.ListUsers;
using Moq;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.User;

public class ListUsersUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPageSizeIsInvalid()
    {
        var useCase = new ListUsersUseCase(new Mock<IUserRepository>().Object);

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
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(repository =>
                repository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>(), 1, 1)
            )
            .ReturnsAsync([users[0]]);
        repository
            .Setup(repository =>
                repository.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())
            )
            .ReturnsAsync(2);
        var useCase = new ListUsersUseCase(repository.Object);

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
        repository.Verify(
            repository =>
                repository.GetAllAsync(
                    It.Is<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>(predicate =>
                        predicate.Compile()(users[0]) && !predicate.Compile()(users[1]) && predicate.Compile()(users[2])
                    ),
                    1,
                    1
                ),
            Times.Once
        );
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
