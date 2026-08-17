using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MeusPila.Domain.Interfaces;

namespace MeusPila.Domain.Entities;

public class User : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(100)]
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public required string Role { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? UpdatedById { get; set; }

    [ForeignKey("UpdatedById")]
    public User? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }
}
