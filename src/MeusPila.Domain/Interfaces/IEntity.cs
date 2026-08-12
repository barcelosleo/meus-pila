namespace MeusPila.Domain.Entities;

public interface IEntity
{
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
}
