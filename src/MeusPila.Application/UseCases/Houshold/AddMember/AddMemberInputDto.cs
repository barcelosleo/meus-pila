namespace MeusPila.Application.UseCases.Houshold.AddMember;

using System.ComponentModel.DataAnnotations;

public record AddMemberInputDto
{
    [Required(ErrorMessage = "O id do grupo é obrigatório")]
    public required string HouseholdId { get; init; }

    [Required(ErrorMessage = "O id do membro é obrigatório")]
    public required string MemberId { get; init; }

    [Required(ErrorMessage = "O id de quem atualizou é obrigatório")]
    public required string UpdatedById { get; init; }
}
