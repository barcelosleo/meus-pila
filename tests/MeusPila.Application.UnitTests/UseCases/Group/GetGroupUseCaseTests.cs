using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.Group.GetGroup;
using Moq;

namespace MeusPila.Application.UnitTests.UseCases.Group;

public class GetGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnHouseholdWithMembers()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var member = GroupTestData.CreateUser("Member");
        var household = GroupTestData.CreateGroup(owner);
        var groupRepository = new Mock<IGroupRepository>();
        groupRepository.Setup(repository => repository.GetByIdAsync(household.Id)).ReturnsAsync(household);
        groupRepository
            .Setup(repository => repository.GetMembers(household))
            .ReturnsAsync(new[] { member }.AsEnumerable());
        var useCase = new GetGroupUseCase(groupRepository.Object);

        var output = await useCase.ExecuteAsync(
            new GetGroupInputDto { GroupId = household.Id.ToString(), RequesterId = owner.Id.ToString() }
        );

        Assert.Equal(household.Name, output.Name);
        Assert.Single(output.Members);
        Assert.Equal(member.Name, output.Members[0].Name);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenHouseholdDoesNotExist()
    {
        var groupRepository = new Mock<IGroupRepository>();
        groupRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((MeusPila.Domain.Entities.Group?)null);
        var useCase = new GetGroupUseCase(groupRepository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new GetGroupInputDto { GroupId = Guid.NewGuid().ToString(), RequesterId = Guid.NewGuid().ToString() }
            )
        );
    }
}
