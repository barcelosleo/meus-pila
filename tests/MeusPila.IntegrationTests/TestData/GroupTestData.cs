using MeusPila.Infrastructure.Data;
using GroupEntity = MeusPila.Domain.Entities.Group;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.IntegrationTests.TestData;

internal static class GroupTestData
{
    public static UserEntity CreateUser(string name, string role = "User") =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = $"{name.ToLower()}-{Guid.NewGuid():N}@mail.com",
            PasswordHash = string.Empty,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

    public static GroupEntity CreateGroup(UserEntity owner, string name = "Home") =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            OwnerId = owner.Id,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
        };

    public static async Task SeedAsync(ApplicationDbContext context, params UserEntity[] users)
    {
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }

    public static async Task SeedAsync(ApplicationDbContext context, GroupEntity group)
    {
        context.Groups.Add(group);
        await context.SaveChangesAsync();
    }
}
