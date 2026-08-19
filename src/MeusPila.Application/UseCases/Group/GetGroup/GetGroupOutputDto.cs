namespace MeusPila.Application.UseCases.Group.GetGroup;

public record MemberDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}

public record GetGroupOutputDto
{
    public required string Id { get; init; } = string.Empty;
    public required string Name { get; init; } = string.Empty;
    public required DateTime CreatedAt { get; init; }
    public string? CreatedById { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
    public required List<MemberDto> Members { get; init; }
}
