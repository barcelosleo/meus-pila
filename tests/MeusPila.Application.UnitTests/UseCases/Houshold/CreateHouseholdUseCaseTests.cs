using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.Houshold.CreateHousehold;

namespace MeusPila.Application.UnitTests.UseCases.Houshold;

public class CreateHouseholdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPromoteOwnerAndCreateHousehold()
    {
        var owner = HouseholdTestData.CreateUser("Owner");
        var repository = new InMemoryHouseholdRepository();
        var useCase = new CreateHouseholdUseCase(new InMemoryUserRepository([owner]), repository);

        var output = await useCase.ExecuteAsync(
            new CreateHouseholdInputDto { Name = "Family", OwnerId = owner.Id.ToString() }
        );

        Assert.Equal("Family", output.Name);
        Assert.Equal(owner.Id.ToString(), output.OwnerId);
        Assert.Equal("HouseholdManager", owner.Role);
        Assert.Equal(1, repository.AddCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenOwnerDoesNotExist()
    {
        var useCase = new CreateHouseholdUseCase(new InMemoryUserRepository(), new InMemoryHouseholdRepository());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(new CreateHouseholdInputDto { Name = "Family", OwnerId = Guid.NewGuid().ToString() })
        );
    }
}
