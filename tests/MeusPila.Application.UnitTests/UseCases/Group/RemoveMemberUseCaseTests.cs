using MeusPila.Application.Interfaces;
using MeusPila.Application.UnitTests.TestDoubles;
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
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(owner.Id)).ReturnsAsync(owner);
        userRepository.Setup(repository => repository.GetByIdAsync(member.Id)).ReturnsAsync(member);
        var household = GroupTestData.CreateGroup(owner);
        household.Members.Add(
            new MeusPila.Domain.Entities.GroupUser
            {
                GroupId = household.Id,
                Group = household,
                UserId = member.Id,
                User = member,
            }
        );
        var householdRepository = new InMemoryHouseholdRepository([household]);
        var useCase = new RemoveMemberUseCase(householdRepository, userRepository.Object);

        var output = await useCase.ExecuteAsync(
            new RemoveMemberInputDto
            {
                GroupId = household.Id.ToString(),
                MemberId = member.Id.ToString(),
                UpdatedById = owner.Id.ToString(),
            }
        );

        Assert.Empty(output.Members);
        Assert.Equal(1, householdRepository.RemoveMemberCalls);
        Assert.Equal(1, householdRepository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenRegularUserRequestsRemoval()
    {
        var owner = GroupTestData.CreateUser("Owner", "GroupManager");
        var requester = GroupTestData.CreateUser("Requester");
        var member = GroupTestData.CreateUser("Member");
        var household = GroupTestData.CreateGroup(owner);
        var householdRepository = new InMemoryHouseholdRepository([household]);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(requester.Id)).ReturnsAsync(requester);
        var useCase = new RemoveMemberUseCase(householdRepository, userRepository.Object);

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
        Assert.Equal(0, householdRepository.RemoveMemberCalls);
    }
}
