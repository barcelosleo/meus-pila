using MeusPila.Domain.Entities;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.UseCases.Houshold;

internal static class HouseholdTestData
{
    public static UserEntity CreateUser(string name, string role = "User") =>
        new()
        {
            Name = name,
            Email = $"{name.ToLower()}@mail.com",
            PasswordHash = string.Empty,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

    public static Household CreateHousehold(UserEntity owner, string name = "Home") =>
        new()
        {
            Name = name,
            OwnerId = owner.Id,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
        };
}
