namespace MeusPila.Application.UseCases.Group.RemoveMember;

public record MemberDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}

public record RemoveMemberOutputDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    public string? CreatedById { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedById { get; init; }
    public required List<MemberDto> Members { get; init; }
}
