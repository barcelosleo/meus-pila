using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a service interface for hashing and verifying user passwords.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hashes the specified password for the given user.
    /// </summary>
    /// <param name="user">The user for whom to hash the password.</param>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(UserEntity user, string password);

    /// <summary>
    /// Verifies the specified password for the given user.
    /// </summary>
    /// <param name="user">The user for whom to verify the password.</param>
    /// <param name="password">The password to verify.</param>
    /// <returns>True if the password is correct; otherwise, false.</returns>
    bool VerifyPassword(UserEntity user, string password);
}
