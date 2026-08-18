using MeusPila.Application.Interfaces;
using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.Houshold.RemoveMember;
using Moq;

namespace MeusPila.Application.UnitTests.UseCases.Houshold;

public class RemoveMemberUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldRemoveMember_WhenRequesterIsOwnerManager()
    {
        var owner = HouseholdTestData.CreateUser("Owner", "HouseholdManager");
        var member = HouseholdTestData.CreateUser("Member");
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(owner.Id)).ReturnsAsync(owner);
        userRepository.Setup(repository => repository.GetByIdAsync(member.Id)).ReturnsAsync(member);
        var household = HouseholdTestData.CreateHousehold(owner);
        household.Members.Add(
            new MeusPila.Domain.Entities.HouseholdUser
            {
                HouseholdId = household.Id,
                Household = household,
                UserId = member.Id,
                User = member,
            }
        );
        var householdRepository = new InMemoryHouseholdRepository([household]);
        var useCase = new RemoveMemberUseCase(householdRepository, userRepository.Object);

        var output = await useCase.ExecuteAsync(
            new RemoveMemberInputDto
            {
                HouseholdId = household.Id.ToString(),
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
        var owner = HouseholdTestData.CreateUser("Owner", "HouseholdManager");
        var requester = HouseholdTestData.CreateUser("Requester");
        var member = HouseholdTestData.CreateUser("Member");
        var household = HouseholdTestData.CreateHousehold(owner);
        var householdRepository = new InMemoryHouseholdRepository([household]);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(requester.Id)).ReturnsAsync(requester);
        var useCase = new RemoveMemberUseCase(householdRepository, userRepository.Object);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.ExecuteAsync(
                new RemoveMemberInputDto
                {
                    HouseholdId = household.Id.ToString(),
                    MemberId = member.Id.ToString(),
                    UpdatedById = requester.Id.ToString(),
                }
            )
        );
        Assert.Equal(0, householdRepository.RemoveMemberCalls);
    }
}
