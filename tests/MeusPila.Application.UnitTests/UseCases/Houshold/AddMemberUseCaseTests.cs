using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.Houshold.AddMember;

namespace MeusPila.Application.UnitTests.UseCases.Houshold;

public class AddMemberUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldAddMemberAndReturnMembers()
    {
        var requester = HouseholdTestData.CreateUser("Requester", "HouseholdManager");
        var member = HouseholdTestData.CreateUser("Member");
        var household = HouseholdTestData.CreateHousehold(requester);
        var householdRepository = new InMemoryHouseholdRepository([household]);
        var useCase = new AddMemberUseCase(householdRepository, new InMemoryUserRepository([requester, member]));

        var output = await useCase.ExecuteAsync(
            new AddMemberInputDto
            {
                HouseholdId = household.Id.ToString(),
                MemberId = member.Id.ToString(),
                UpdatedById = requester.Id.ToString(),
            }
        );

        Assert.Equal(household.Id.ToString(), output.HouseholdId);
        Assert.Single(output.Members);
        Assert.Equal(member.Id.ToString(), output.Members[0].Id);
        Assert.Equal(1, householdRepository.AddMemberCalls);
        Assert.Equal(1, householdRepository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenMemberDoesNotExist()
    {
        var requester = HouseholdTestData.CreateUser("Requester");
        var household = HouseholdTestData.CreateHousehold(requester);
        var useCase = new AddMemberUseCase(
            new InMemoryHouseholdRepository([household]),
            new InMemoryUserRepository([requester])
        );

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new AddMemberInputDto
                {
                    HouseholdId = household.Id.ToString(),
                    MemberId = Guid.NewGuid().ToString(),
                    UpdatedById = requester.Id.ToString(),
                }
            )
        );
    }
}
