using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.User.UpdateUser;

public class UpdateUserUseCase : IUseCase<UpdateUserInputDto, UpdateUserOutputDto>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UpdateUserOutputDto> ExecuteAsync(UpdateUserInputDto input)
    {
        UserEntity user = await _userRepository.GetByIdAsync(Guid.Parse(input.Id));
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        if (!string.IsNullOrEmpty(input.Name))
        {
            user.Name = input.Name;
        }

        if (!string.IsNullOrEmpty(input.Email))
        {
            bool emailExists = await _userRepository.CountAsync(u => u.Email == input.Email && u.Id != user.Id) > 0;
            if (emailExists)
            {
                throw new InvalidOperationException("O email informado já está em uso.");
            }
            user.Email = input.Email;
        }

        if (!string.IsNullOrEmpty(input.NewPassword))
        {
            if (string.IsNullOrEmpty(input.CurrentPassword))
            {
                throw new InvalidOperationException("A senha atual é obrigatória para alterar a senha.");
            }

            var passwordService = new PasswordService();

            if (!passwordService.VerifyPassword(user, input.CurrentPassword))
            {
                throw new InvalidOperationException("A senha atual informada está incorreta.");
            }

            user.PasswordHash = passwordService.HashPassword(user, input.NewPassword);
        }

        if (input.IsActive.HasValue)
        {
            user.IsActive = input.IsActive.Value;
        }

        await _userRepository.UpdateAsync(user);

        return new UpdateUserOutputDto
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
        };
    }
}
