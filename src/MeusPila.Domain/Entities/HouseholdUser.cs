using System.ComponentModel.DataAnnotations.Schema;

namespace MeusPila.Domain.Entities;

public class HouseholdUser
{
    public required Guid HouseholdId { get; set; }

    [ForeignKey("HouseholdId")]
    public required Household Household { get; set; }
    public required Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public required User User { get; set; }
}
