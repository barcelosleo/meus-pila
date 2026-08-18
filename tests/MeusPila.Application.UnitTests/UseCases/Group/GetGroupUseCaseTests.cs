using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.Group.GetGroup;

namespace MeusPila.Application.UnitTests.UseCases.Group;

public class GetGroupUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnHouseholdWithMembers()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var member = GroupTestData.CreateUser("Member");
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
        var useCase = new GetGroupUseCase(new InMemoryHouseholdRepository([household]));

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
        var useCase = new GetGroupUseCase(new InMemoryHouseholdRepository());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new GetGroupInputDto { GroupId = Guid.NewGuid().ToString(), RequesterId = Guid.NewGuid().ToString() }
            )
        );
    }
}
