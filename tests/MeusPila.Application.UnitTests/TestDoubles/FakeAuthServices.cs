using MeusPila.Application.Interfaces;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UnitTests.TestDoubles;

internal sealed class FakeJwtService : IJwtService
{
    public string AccessTokenToReturn { get; set; } = "access-token";
    public string RefreshTokenToReturn { get; set; } = "refresh-token";

    public string GenerateToken(UserEntity user)
    {
        return AccessTokenToReturn;
    }

    public string GenerateRefreshToken()
    {
        return RefreshTokenToReturn;
    }
}

internal sealed class FakePasswordService : IPasswordService
{
    public bool VerifyResult { get; set; } = true;

    public string HashPassword(UserEntity user, string password)
    {
        return "hash";
    }

    public bool VerifyPassword(UserEntity user, string password)
    {
        return VerifyResult;
    }
}
