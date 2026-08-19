using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Group.CreateGroup;

public record CreateGroupInputDto
{
    public required string Name { get; init; } = string.Empty;

    public required string OwnerId { get; init; }

    public required string CreatedById { get; init; }
}

public record CreateGroupQueryDto
{
    [Required(ErrorMessage = "O nome do grupo é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome do grupo deve ter no máximo 100 caracteres")]
    public required string Name { get; init; } = string.Empty;
}
