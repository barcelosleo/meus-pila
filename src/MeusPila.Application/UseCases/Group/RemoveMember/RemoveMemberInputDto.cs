using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Group.RemoveMember;

public record RemoveMemberInputDto
{
    [Required(ErrorMessage = "O id do grupo é obrigatório")]
    public required string GroupId { get; init; }

    [Required(ErrorMessage = "O id do membro é obrigatório")]
    public required string MemberId { get; init; }

    [Required(ErrorMessage = "O id de quem requisitou é obrigatório")]
    public required string UpdatedById { get; init; }
}
