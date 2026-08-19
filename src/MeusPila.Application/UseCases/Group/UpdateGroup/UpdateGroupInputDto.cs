using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Group.UpdateGroup;

public record UpdateGroupInputDto
{
    public required string Id;

    public string? Name;

    public string? UpdatedById;
}

public record UpdateGroupQueryDto
{
    [StringLength(100, ErrorMessage = "O nome do grupo deve ter no máximo 100 caracteres")]
    public string? Name { get; init; }
}
