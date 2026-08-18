using System.ComponentModel.DataAnnotations;

namespace MeusPila.Application.UseCases.Group.UpdateGroup;

public record UpdateGroupInputDto
{
    public required string Id;

    public string? Name = string.Empty;

    public string? UpdatedById = string.Empty;
}

public record UpdateGroupQueryDto
{
    [StringLength(100, ErrorMessage = "O nome do grupo deve ter no máximo 100 caracteres")]
    public string? Name = string.Empty;
}
