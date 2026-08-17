using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.Houshold.GetHousehold;

namespace MeusPila.Application.UnitTests.UseCases.Houshold;

public class GetHouseholdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnHouseholdWithMembers()
    {
        var owner = HouseholdTestData.CreateUser("Owner");
        var member = HouseholdTestData.CreateUser("Member");
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
        var useCase = new GetHouseholdUseCase(new InMemoryHouseholdRepository([household]));

        var output = await useCase.ExecuteAsync(
            new GetHouseholdInputDto { HouseholdId = household.Id.ToString(), RequesterId = owner.Id.ToString() }
        );

        Assert.Equal(household.Name, output.Name);
        Assert.Single(output.Members);
        Assert.Equal(member.Name, output.Members[0].Name);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenHouseholdDoesNotExist()
    {
        var useCase = new GetHouseholdUseCase(new InMemoryHouseholdRepository());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new GetHouseholdInputDto
                {
                    HouseholdId = Guid.NewGuid().ToString(),
                    RequesterId = Guid.NewGuid().ToString(),
                }
            )
        );
    }
}
