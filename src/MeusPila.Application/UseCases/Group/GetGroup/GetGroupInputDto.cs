using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Group.GetGroup;

public record GetGroupInputDto
{
    [Required(ErrorMessage = "O id do grupo é obrigatório")]
    public required string GroupId { get; init; }

    [Required(ErrorMessage = "O id de quem requisitou é obrigatório")]
    public required string RequesterId { get; init; }
}
