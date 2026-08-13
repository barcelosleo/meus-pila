using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a service interface for generating JSON Web Tokens (JWT) and refresh tokens for user authentication.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the JWT.</param>
    /// <returns>The generated JSON Web Token (JWT).</returns>
    string GenerateToken(UserEntity user);

    /// <summary>
    /// Generates a refresh token for the user.
    /// </summary>
    /// <returns>The generated refresh token.</returns>
    string GenerateRefreshToken();
}
