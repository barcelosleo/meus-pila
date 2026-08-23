using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.User.UpdateUser;

public interface IUpdateUserUseCase : IUseCase<UpdateUserInputDto, UpdateUserOutputDto> { }

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UpdateUserOutputDto> ExecuteAsync(UpdateUserInputDto input)
    {
        UserEntity? user = await _userRepository.GetByIdAsync(Guid.Parse(input.Id));
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        var updaterId = Guid.Parse(input.UpdatedById);
        UserEntity? updater = await _userRepository.GetByIdAsync(updaterId);
        if (updater == null)
        {
            throw new InvalidOperationException("O usuário que está atualizando o usuário não existe.");
        }

        if (updater.Role != "Admin" && updater.Id != user.Id)
        {
            throw new InvalidOperationException("Apenas administradores podem atualizar outros usuários.");
        }

        user.UpdatedById = updaterId;

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
            user.Email = input.Email.ToLower();
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

        user.UpdatedAt = DateTime.UtcNow;

        UserEntity updatedUser = await _userRepository.UpdateAsync(user);

        return new UpdateUserOutputDto
        {
            Id = updatedUser.Id.ToString(),
            Name = updatedUser.Name,
            Email = updatedUser.Email,
            IsActive = updatedUser.IsActive,
        };
    }
}
