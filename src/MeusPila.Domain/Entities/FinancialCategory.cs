namespace MeusPila.Domain.Entities;

public class FinancialCategory : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? OwenerId { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public FinancialCategory(string name, Guid? parentCategoryId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        ParentCategoryId = parentCategoryId;
    }
}
