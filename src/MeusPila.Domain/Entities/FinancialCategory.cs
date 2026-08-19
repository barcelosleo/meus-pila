using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class FinancialCategory : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }
    public Guid? OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public User? Owner { get; set; }
    public Guid? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    public FinancialCategory? ParentCategory { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }

    [ForeignKey("UpdatedById")]
    public User? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }
}
