namespace MeusPila.Domain.Interfaces;

public interface IEntity
{
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
    Guid? CreatedById { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedById { get; set; }
}
