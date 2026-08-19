namespace MeusPila.Application.UseCases.Group.AddMember;

public record MemberDto
{
    public required string Id;
    public required string Name;
}

public record AddMemberOutputDto
{
    public required string GroupId = string.Empty;
    public required string Name = string.Empty;
    public DateTime? UpdatedAt;
    public string? UpdatedById;
    public required List<MemberDto> Members;
}
