namespace MeusPila.Application.UseCases.Group.AddMember;

public record MemberDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}

public record AddMemberOutputDto
{
    public required string GroupId { get; init; } = string.Empty;
    public required string Name { get; init; } = string.Empty;
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
    public required List<MemberDto> Members { get; init; }
}
