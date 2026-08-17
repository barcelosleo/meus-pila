using MeusPila.Application.Interfaces;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.User.RefreshToken;

public interface IRefreshTokenUseCase : IUseCase<RefreshTokenInputDto, RefreshTokenOutputDto> { }

public class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RefreshTokenUseCase(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<RefreshTokenOutputDto> ExecuteAsync(RefreshTokenInputDto input)
    {
        UserEntity? user = await _userRepository.GetByIdAsync(input.UserId);
        if (user is null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        if (user.RefreshToken != input.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Refresh token inválido ou expirado.");
        }

        string newRefreshToken = _jwtService.GenerateRefreshToken();
        string newAccessToken = _jwtService.GenerateToken(user);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);

        return new RefreshTokenOutputDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
    }
}
