using System.Net.Http.Headers;
using System.Net.Http.Json;
using MeusPila.Application.Interfaces;
using MeusPila.Application.UseCases.Group.CreateGroup;
using MeusPila.Application.UseCases.Group.GetGroup;
using MeusPila.IntegrationTests.Fixtures;
using MeusPila.IntegrationTests.TestData;
using Microsoft.Extensions.DependencyInjection;

namespace MeusPila.IntegrationTests.Controllers;

[Collection(DatabaseCollection.Name)]
public class GroupControllerTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private ApiFactory _apiFactory = null!;

    public GroupControllerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetAsync();
        _apiFactory = new ApiFactory(_fixture.ConnectionString);
    }

    public async Task DisposeAsync() => await _apiFactory.DisposeAsync();

    [Fact]
    public async Task CreateGroup_ShouldPersistGroup_AndGetGroup_ShouldReturnIt()
    {
        var owner = GroupTestData.CreateUser("Owner");
        await using (var context = _fixture.CreateDbContext())
        {
            await GroupTestData.SeedAsync(context, owner);
        }

        using var client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(owner));

        var createResponse = await client.PostAsJsonAsync("/api/group", new { Name = "Family" });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<CreateGroupOutputDto>();
        Assert.NotNull(created);
        Assert.Equal("Family", created!.Name);

        var getResponse = await client.GetAsync($"/api/group/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<GetGroupOutputDto>();
        Assert.NotNull(fetched);
        Assert.Equal("Family", fetched!.Name);
    }

    [Fact]
    public async Task GetGroup_ShouldReturnBadRequest_WhenGroupDoesNotExist()
    {
        var owner = GroupTestData.CreateUser("Owner");
        await using (var context = _fixture.CreateDbContext())
        {
            await GroupTestData.SeedAsync(context, owner);
        }

        using var client = _apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(owner));

        var response = await client.GetAsync($"/api/group/{Guid.NewGuid()}");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    private string CreateToken(MeusPila.Domain.Entities.User user)
    {
        using var scope = _apiFactory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IJwtService>().GenerateToken(user);
    }
}
