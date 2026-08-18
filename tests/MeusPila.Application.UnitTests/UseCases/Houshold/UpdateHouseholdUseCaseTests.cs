using MeusPila.Application.Interfaces;
using MeusPila.Application.UnitTests.TestDoubles;
using MeusPila.Application.UseCases.Houshold.UpdateHousehold;
using Moq;

namespace MeusPila.Application.UnitTests.UseCases.Houshold;

public class UpdateHouseholdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldUpdateNameAndUpdater()
    {
        var owner = HouseholdTestData.CreateUser("Owner");
        var updater = HouseholdTestData.CreateUser("Updater");
        var household = HouseholdTestData.CreateHousehold(owner);
        var householdRepository = new InMemoryHouseholdRepository([household]);
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(repository => repository.GetByIdAsync(updater.Id)).ReturnsAsync(updater);
        var useCase = new UpdateHouseholdUseCase(householdRepository, userRepository.Object);

        var output = await useCase.ExecuteAsync(
            new UpdateHouseholdInputDto
            {
                Id = household.Id.ToString(),
                Name = "Updated Home",
                UpdatedById = updater.Id.ToString(),
            }
        );

        Assert.Equal("Updated Home", output.Name);
        Assert.Equal(updater.Id.ToString(), output.UpdatedById);
        Assert.Equal(1, householdRepository.UpdateCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUpdaterDoesNotExist()
    {
        var owner = HouseholdTestData.CreateUser("Owner");
        var household = HouseholdTestData.CreateHousehold(owner);
        var missingUpdaterId = Guid.NewGuid();
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.GetByIdAsync(missingUpdaterId))
            .ReturnsAsync((MeusPila.Domain.Entities.User?)null);
        var useCase = new UpdateHouseholdUseCase(new InMemoryHouseholdRepository([household]), userRepository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                new UpdateHouseholdInputDto { Id = household.Id.ToString(), UpdatedById = missingUpdaterId.ToString() }
            )
        );
    }
}
