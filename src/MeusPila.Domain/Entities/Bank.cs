namespace MeusPila.Domain.Entities;

public class Bank : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Bank(string name, string code, DateTime? createdAt = null, DateTime? updatedAt = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Code = code;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }
}
