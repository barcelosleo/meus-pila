using MeusPila.Application.Interfaces;
using MeusPila.Application.Services;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.UseCases.User.CreateUser;

public class CreateUserUseCase : IUseCase<CreateUserInputDto, CreateUserOutputDto>
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

        var user = new UserEntity(input.Name, input.Email.ToLowerInvariant(), input.Password);
        var passwordService = new PasswordService();
        user.PasswordHash = passwordService.HashPassword(user, input.Password);

        var createUser = await _userRepository.AddAsync(user);

        return new CreateUserOutputDto
        {
            Id = createUser.Id.ToString(),
            Name = createUser.Name,
            Email = createUser.Email,
        };
    }
}
