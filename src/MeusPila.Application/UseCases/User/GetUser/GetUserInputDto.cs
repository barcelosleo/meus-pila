using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.User.GetUser;

public record GetUserInputDto
{
    [Required(ErrorMessage = "O id do usuário é obrigatório")]
    public required string Id { get; init; }

    [Required(ErrorMessage = "O id de quem fez a requisição é obrigatório")]
    public required string RequesterId { get; init; }
}
