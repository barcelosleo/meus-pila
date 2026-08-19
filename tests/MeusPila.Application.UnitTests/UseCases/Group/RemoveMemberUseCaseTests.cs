using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.Group.RemoveMember;
using Moq;

namespace MeusPila.Application.UnitTests.UseCases.Group;

public class RemoveMemberUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldRemoveMember_WhenRequesterIsOwnerManager()
    {
        var owner = GroupTestData.CreateUser("Owner", "GroupManager");
        var member = GroupTestData.CreateUser("Member");
        var household = GroupTestData.CreateGroup(owner);
        var groupRepository = new Mock<IGroupRepository>();
        groupRepository.Setup(repository => repository.GetByIdAsync(household.Id)).ReturnsAsync(household);
        groupRepository
            .Setup(repository => repository.UpdateAsync(household))
            .ReturnsAsync((MeusPila.Domain.Entities.Group group) => group);
        groupRepository
            .Setup(repository => repository.GetMembers(household))
            .ReturnsAsync(Enumerable.Empty<MeusPila.Domain.Entities.User>());
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(owner.Id)).ReturnsAsync(owner);
        userRepository.Setup(repository => repository.GetByIdAsync(member.Id)).ReturnsAsync(member);
        var useCase = new RemoveMemberUseCase(groupRepository.Object, userRepository.Object);

        var output = await useCase.ExecuteAsync(
            new RemoveMemberInputDto
            {
                GroupId = household.Id.ToString(),
                MemberId = member.Id.ToString(),
                UpdatedById = owner.Id.ToString(),
            }
        );

        Assert.Empty(output.Members);
        groupRepository.Verify(repository => repository.RemoveMemberAsync(household, member), Times.Once);
        groupRepository.Verify(repository => repository.UpdateAsync(household), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenRegularUserRequestsRemoval()
    {
        var owner = GroupTestData.CreateUser("Owner", "GroupManager");
        var requester = GroupTestData.CreateUser("Requester");
        var member = GroupTestData.CreateUser("Member");
        var household = GroupTestData.CreateGroup(owner);
        var groupRepository = new Mock<IGroupRepository>();
        groupRepository.Setup(repository => repository.GetByIdAsync(household.Id)).ReturnsAsync(household);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(requester.Id)).ReturnsAsync(requester);
        var useCase = new RemoveMemberUseCase(groupRepository.Object, userRepository.Object);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.ExecuteAsync(
                new RemoveMemberInputDto
                {
                    GroupId = household.Id.ToString(),
                    MemberId = member.Id.ToString(),
                    UpdatedById = requester.Id.ToString(),
                }
            )
        );
        groupRepository.Verify(
            repository =>
                repository.RemoveMemberAsync(
                    It.IsAny<MeusPila.Domain.Entities.Group>(),
                    It.IsAny<MeusPila.Domain.Entities.User>()
                ),
            Times.Never
        );
    }
}
