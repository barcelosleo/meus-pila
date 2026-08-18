using MeusPila.Application.Interfaces;

namespace MeusPila.Application.UseCases.User.RemoveUser;

public interface IRemoveUserUseCase : IUseCase<RemoveUserInputDto, object> { }

public class RemoveUserUseCase : IRemoveUserUseCase
{
    private readonly IUserRepository _userRepository;

    public RemoveUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<object> ExecuteAsync(RemoveUserInputDto input)
    {
        Guid userId = Guid.Parse(input.Id);
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"O usuário com ID {input.Id} não foi encontrado.");
        }

        Guid requesterId = Guid.Parse(input.RequesterId);
        var requester = await _userRepository.GetByIdAsync(requesterId);
        if (requester == null)
        {
            throw new ArgumentException(
                $"O usuário que está fazendo a requisição com ID {input.RequesterId} não foi encontrado."
            );
        }

        if (requester.Role != "Admin")
        {
            throw new UnauthorizedAccessException("O usuário não tem permissão para remover outro usuário.");
        }

        await _userRepository.DeleteAsync(user);

        return null!;
    }
}
