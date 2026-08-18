namespace MeusPila.Application.UseCases.Group.RemoveGroup;

using System.ComponentModel.DataAnnotations;

public record RemoveGroupInputDto
{
    [Required(ErrorMessage = "O id do grupo é obrigatório")]
    public required string GroupId { get; init; }

    [Required(ErrorMessage = "O id de quem está solicitando a remoção é obrigatório")]
    public required string RequesterId { get; init; }
}
