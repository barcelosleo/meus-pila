using MeusPila.Application.Interfaces;

namespace MeusPila.Application.UseCases.User.GetUser;

public interface IGetUserUseCase : IUseCase<GetUserInputDto, GetUserOutputDto> { }

public class GetUserUseCase : IGetUserUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserOutputDto> ExecuteAsync(GetUserInputDto input)
    {
        Guid requesterId = Guid.Parse(input.RequesterId);
        var requester = await _userRepository.GetByIdAsync(requesterId);
        if (requester == null)
        {
            throw new ArgumentException(
                $"O usuário que está fazendo a requisição com ID {input.RequesterId} não foi encontrado."
            );
        }

        if (requester.Role != "Admin" && requester.Role != "HouseholdManager")
        {
            throw new UnauthorizedAccessException(
                "O usuário não tem permissão para acessar as informações de outro usuário."
            );
        }

        Guid userId = Guid.Parse(input.Id);
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"O usuário com ID {input.Id} não foi encontrado.");
        }

        if (requester.Role == "HouseholdManager")
        {
            var requesterCanSeeUser = await _userRepository.UserCanViewHouseholdMembersAsync(requesterId, userId);
            if (!requesterCanSeeUser)
            {
                throw new UnauthorizedAccessException(
                    "O usuário não tem permissão para acessar as informações de outro usuário."
                );
            }
        }

        return new GetUserOutputDto
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            CreatedById = user.CreatedById?.ToString() ?? "",
            UpdatedAt = user.UpdatedAt,
            UpdatedById = user.UpdatedById?.ToString() ?? "",
        };
    }
}
