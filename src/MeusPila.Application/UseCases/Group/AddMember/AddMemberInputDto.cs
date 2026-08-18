namespace MeusPila.Application.UseCases.Group.AddMember;

using System.ComponentModel.DataAnnotations;

public record AddMemberInputDto
{
    [Required(ErrorMessage = "O id do grupo é obrigatório")]
    public required string GroupId { get; init; }

    [Required(ErrorMessage = "O id do membro é obrigatório")]
    public required string MemberId { get; init; }

    [Required(ErrorMessage = "O id de quem atualizou é obrigatório")]
    public required string UpdatedById { get; init; }
}
