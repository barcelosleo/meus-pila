using MeusPila.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<UserEntity> _passwordHasher;

    public PasswordService()
    {
        _passwordHasher = new PasswordHasher<UserEntity>();
    }

    public string HashPassword(UserEntity user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(UserEntity user, string password)
    {
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new ArgumentException("Usuário ou hash de senha inválido.");
        }
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result switch
        {
            PasswordVerificationResult.Success => true,
            PasswordVerificationResult.SuccessRehashNeeded => true,
            PasswordVerificationResult.Failed => false,
            _ => false,
        };
    }
}
