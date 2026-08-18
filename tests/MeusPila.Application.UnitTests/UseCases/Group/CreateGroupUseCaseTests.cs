using MeusPila.Application.Interfaces;
using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.Group.CreateGroup;
using Moq;

namespace MeusPila.Application.UnitTests.UseCases.Group;

public class CreateGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPromoteOwnerAndCreateGroup()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var repository = new InMemoryHouseholdRepository();
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(owner.Id)).ReturnsAsync(owner);
        var useCase = new CreateGroupUseCase(userRepository.Object, repository);

        var output = await useCase.ExecuteAsync(
            new CreateGroupInputDto
            {
                Name = "Family",
                OwnerId = owner.Id.ToString(),
                CreatedById = owner.Id.ToString(),
            }
        );

        Assert.Equal("Family", output.Name);
        Assert.Equal(owner.Id.ToString(), output.OwnerId);
        Assert.Equal("HouseholdManager", owner.Role);
        Assert.Equal(1, repository.AddCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenOwnerDoesNotExist()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((MeusPila.Domain.Entities.User?)null);
        var useCase = new CreateGroupUseCase(userRepository.Object, new InMemoryHouseholdRepository());

        var createdById = Guid.NewGuid().ToString();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new CreateGroupInputDto
                {
                    Name = "Family",
                    OwnerId = Guid.NewGuid().ToString(),
                    CreatedById = createdById,
                }
            )
        );
    }
}
