using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.Admin.CreateUser;

public interface ICreateUserUseCase : IUseCase<CreateUserInputDto, CreateUserOutputDto> { }

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateUserUseCase(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CreateUserOutputDto> ExecuteAsync(CreateUserInputDto input)
    {
        bool userExists = await _userRepository.CountAsync(u => u.Email == input.Email) > 0;
        if (userExists)
        {
            throw new InvalidOperationException("O email informado já está em uso.");
        }

        Guid requesterId = Guid.Parse(_currentUserService.UserId);
        UserEntity? requester = await _userRepository.GetByIdAsync(requesterId);
        if (requester == null)
        {
            throw new InvalidOperationException("O usuário que está criando o novo usuário não existe.");
        }
        if (requester.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Apenas usuários com a função 'Admin' podem criar novos usuários.");
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
