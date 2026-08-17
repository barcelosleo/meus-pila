using MeusPila.Application.Interfaces;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.User.AuthenticateUser;

public interface IAuthenticateUserUseCase : IUseCase<AuthenticateUserInputDto, AuthenticateUserOutputDto> { }

public class AuthenticateUserUseCase : IAuthenticateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthenticateUserUseCase(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService
    )
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<AuthenticateUserOutputDto> ExecuteAsync(AuthenticateUserInputDto input)
    {
        UserEntity? user = await _userRepository.FindAsync(u => u.Email == input.Email);
        if (user == null || !_passwordService.VerifyPassword(user, input.Password))
        {
            throw new InvalidOperationException("Email ou senha inválidos.");
        }
        if (!user.IsActive)
        {
            throw new InvalidOperationException("Usuário inativo.");
        }

        string refreshToken = _jwtService.GenerateRefreshToken();
        string accessToken = _jwtService.GenerateToken(user);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);

        return new AuthenticateUserOutputDto
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}
