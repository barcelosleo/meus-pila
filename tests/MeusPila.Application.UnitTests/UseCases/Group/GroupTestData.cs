using MeusPila.Domain.Entities;
using GroupEntity = MeusPila.Domain.Entities.Group;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.Group;

internal static class GroupTestData
{
    public static UserEntity CreateUser(string name, string role = "User") =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = $"{name.ToLower()}@mail.com",
            PasswordHash = string.Empty,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

    public static GroupEntity CreateGroup(UserEntity owner, string name = "Home") =>
        new()
        {
            Name = name,
            OwnerId = owner.Id,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
        };
}
