using MeusPila.Infrastructure.Repositories;
using MeusPila.IntegrationTests.Fixtures;
using MeusPila.IntegrationTests.TestData;

namespace MeusPila.IntegrationTests.Repositories;

[Collection(DatabaseCollection.Name)]
public class GroupRepositoryTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;

    public GroupRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddAsync_ShouldPersistGroup()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var group = GroupTestData.CreateGroup(owner);

        await using var context = _fixture.CreateDbContext();
        await GroupTestData.SeedAsync(context, owner);
        var repository = new GroupRepository(context);

        await repository.AddAsync(group);

        await using var assertContext = _fixture.CreateDbContext();
        var persisted = await new GroupRepository(assertContext).GetByIdAsync(group.Id);
        Assert.NotNull(persisted);
        Assert.Equal(group.Name, persisted!.Name);
    }

    [Fact]
    public async Task AddMemberAsync_And_GetMembers_ShouldRoundTrip()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var member = GroupTestData.CreateUser("Member");
        var group = GroupTestData.CreateGroup(owner);

        await using var context = _fixture.CreateDbContext();
        await GroupTestData.SeedAsync(context, owner, member);
        await GroupTestData.SeedAsync(context, group);
        var repository = new GroupRepository(context);

        await repository.AddMemberAsync(group, member);

        Assert.True(await repository.IsMemberOfGroupAsync(group, member));
        var members = await repository.GetMembers(group);
        Assert.Single(members);
        Assert.Equal(member.Id, members.Single().Id);
    }

    [Fact]
    public async Task RemoveMemberAsync_ShouldRemoveMembership()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var member = GroupTestData.CreateUser("Member");
        var group = GroupTestData.CreateGroup(owner);

        await using var context = _fixture.CreateDbContext();
        await GroupTestData.SeedAsync(context, owner, member);
        await GroupTestData.SeedAsync(context, group);
        var repository = new GroupRepository(context);
        await repository.AddMemberAsync(group, member);

        await repository.RemoveMemberAsync(group, member);

        Assert.False(await repository.IsMemberOfGroupAsync(group, member));
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        var owner = GroupTestData.CreateUser("Owner");
        var group = GroupTestData.CreateGroup(owner);

        await using var context = _fixture.CreateDbContext();
        await GroupTestData.SeedAsync(context, owner);
        await GroupTestData.SeedAsync(context, group);
        var repository = new GroupRepository(context);

        group.Name = "Renamed";
        await repository.UpdateAsync(group);

        await using var assertContext = _fixture.CreateDbContext();
        var persisted = await new GroupRepository(assertContext).GetByIdAsync(group.Id);
        Assert.Equal("Renamed", persisted!.Name);
    }
}
