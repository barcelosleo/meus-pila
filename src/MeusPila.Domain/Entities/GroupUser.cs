using System.ComponentModel.DataAnnotations.Schema;

namespace MeusPila.Domain.Entities;

public class GroupUser
{
    public required Guid GroupId { get; set; }

    [ForeignKey("GroupId")]
    public required Group Group { get; set; }
    public required Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public required User User { get; set; }
}
