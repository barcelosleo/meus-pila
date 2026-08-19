using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.Group.UpdateGroup;
using Moq;

namespace MeusPila.Application.UnitTests.UseCases.Group;

public class UpdateGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldUpdateNameAndUpdater()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var updater = GroupTestData.CreateUser("Updater");
        var household = GroupTestData.CreateGroup(owner);
        var groupRepository = new Mock<IGroupRepository>();
        groupRepository.Setup(repository => repository.GetByIdAsync(household.Id)).ReturnsAsync(household);
        groupRepository
            .Setup(repository => repository.UpdateAsync(household))
            .ReturnsAsync((MeusPila.Domain.Entities.Group group) => group);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(updater.Id)).ReturnsAsync(updater);
        var useCase = new UpdateGroupUseCase(groupRepository.Object, userRepository.Object);

        var output = await useCase.ExecuteAsync(
            new UpdateGroupInputDto
            {
                Id = household.Id.ToString(),
                Name = "Updated Home",
                UpdatedById = updater.Id.ToString(),
            }
        );

        Assert.Equal("Updated Home", output.Name);
        Assert.Equal(updater.Id.ToString(), output.UpdatedById);
        groupRepository.Verify(repository => repository.UpdateAsync(household), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUpdaterDoesNotExist()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var household = GroupTestData.CreateGroup(owner);
        var missingUpdaterId = Guid.NewGuid();
        var groupRepository = new Mock<IGroupRepository>();
        groupRepository.Setup(repository => repository.GetByIdAsync(household.Id)).ReturnsAsync(household);
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetByIdAsync(missingUpdaterId))
            .ReturnsAsync((MeusPila.Domain.Entities.User?)null);
        var useCase = new UpdateGroupUseCase(groupRepository.Object, userRepository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new UpdateGroupInputDto { Id = household.Id.ToString(), UpdatedById = missingUpdaterId.ToString() }
            )
        );
    }
}
