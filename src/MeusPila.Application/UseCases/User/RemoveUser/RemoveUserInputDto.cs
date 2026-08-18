using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.User.RemoveUser;

public record RemoveUserInputDto
{
    [Required(ErrorMessage = "O id do usuário é obrigatório")]
    public required string Id { get; init; }

    [Required(ErrorMessage = "O id do solicitante é obrigatório")]
    public required string RequesterId { get; init; }
}
