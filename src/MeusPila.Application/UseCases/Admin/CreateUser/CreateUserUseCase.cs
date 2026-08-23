using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Admin.CreateUser;

public interface ICreateUserUseCase : IUseCase<CreateUserInputDto, CreateUserOutputDto> { }

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CreateUserOutputDto> ExecuteAsync(CreateUserInputDto input)
    {
        bool userExists = await _userRepository.CountAsync(u => u.Email == input.Email) > 0;
        if (userExists)
        {
            throw new InvalidOperationException("O email informado já está em uso.");
        }

        var user = new UserEntity
        {
            Name = input.Name,
            Email = input.Email.ToLower(),
            IsActive = true,
            PasswordHash = string.Empty, // Will be set after hashing
            Role = input.Role,
            CreatedAt = DateTime.UtcNow,
        };
        var passwordService = new PasswordService();
        user.PasswordHash = passwordService.HashPassword(user, input.Password);

        if (!string.IsNullOrEmpty(input.CreatedById))
        {
            var creatorId = Guid.Parse(input.CreatedById);
            bool creator = await _userRepository.CountAsync(u => u.Id == creatorId) > 0;
            if (!creator)
            {
                throw new InvalidOperationException("O usuário que está criando o novo usuário não existe.");
            }
            user.CreatedById = creatorId;
        }

        user.Role = input.Role;

        var createdUser = await _userRepository.AddAsync(user);

        return new CreateUserOutputDto
        {
            Id = createdUser.Id.ToString(),
            Name = createdUser.Name,
            Email = createdUser.Email,
        };
    }
}
