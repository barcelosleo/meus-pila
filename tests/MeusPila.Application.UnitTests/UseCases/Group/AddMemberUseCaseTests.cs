using MeusPila.Application.Interfaces;
using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.Group.AddMember;
using Moq;

namespace MeusPila.Application.UnitTests.UseCases.Group;

public class AddMemberUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldAddMemberAndReturnMembers()
    {
        var requester = GroupTestData.CreateUser("Requester", "GroupManager");
        var member = GroupTestData.CreateUser("Member");
        var household = GroupTestData.CreateGroup(requester);
        var householdRepository = new InMemoryHouseholdRepository([household]);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(requester.Id)).ReturnsAsync(requester);
        userRepository.Setup(repository => repository.GetByIdAsync(member.Id)).ReturnsAsync(member);
        var useCase = new AddMemberUseCase(householdRepository, userRepository.Object);

        var output = await useCase.ExecuteAsync(
            new AddMemberInputDto
            {
                GroupId = household.Id.ToString(),
                MemberId = member.Id.ToString(),
                UpdatedById = requester.Id.ToString(),
            }
        );

        Assert.Equal(household.Id.ToString(), output.GroupId);
        Assert.Single(output.Members);
        Assert.Equal(member.Id.ToString(), output.Members[0].Id);
        Assert.Equal(1, householdRepository.AddMemberCalls);
        Assert.Equal(1, householdRepository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenMemberDoesNotExist()
    {
        var requester = GroupTestData.CreateUser("Requester");
        var household = GroupTestData.CreateGroup(requester);
        var missingMemberId = Guid.NewGuid();
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(requester.Id)).ReturnsAsync(requester);
        userRepository
            .Setup(repository => repository.GetByIdAsync(missingMemberId))
            .ReturnsAsync((MeusPila.Domain.Entities.User?)null);
        var useCase = new AddMemberUseCase(new InMemoryHouseholdRepository([household]), userRepository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new AddMemberInputDto
                {
                    GroupId = household.Id.ToString(),
                    MemberId = missingMemberId.ToString(),
                    UpdatedById = requester.Id.ToString(),
                }
            )
        );
    }
}
