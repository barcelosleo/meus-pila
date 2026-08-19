using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.Group.CreateGroup;
using Moq;

namespace MeusPila.Application.UnitTests.UseCases.Group;

public class CreateGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPromoteOwnerAndCreateGroup()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var groupRepository = new Mock<IGroupRepository>();
        groupRepository
            .Setup(repository => repository.AddAsync(It.IsAny<MeusPila.Domain.Entities.Group>()))
            .ReturnsAsync((MeusPila.Domain.Entities.Group group) => group);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(owner.Id)).ReturnsAsync(owner);
        var useCase = new CreateGroupUseCase(userRepository.Object, groupRepository.Object);

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
        Assert.Equal("GroupManager", owner.Role);
        groupRepository.Verify(
            repository => repository.AddAsync(It.IsAny<MeusPila.Domain.Entities.Group>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenOwnerDoesNotExist()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((MeusPila.Domain.Entities.User?)null);
        var useCase = new CreateGroupUseCase(userRepository.Object, new Mock<IGroupRepository>().Object);

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
