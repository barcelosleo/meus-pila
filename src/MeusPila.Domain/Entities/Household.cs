using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class Household : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }
    public required Guid OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public required User Owner { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }

    [ForeignKey("UpdatedById")]
    public User? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }
    public ICollection<HouseholdUser> Members { get; set; } = [];
}
