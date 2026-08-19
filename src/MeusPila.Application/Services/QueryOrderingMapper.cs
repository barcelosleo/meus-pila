using MeusPila.Application.Behaviours;
using MeusPila.Application.Interfaces;

namespace MeusPila.Application.Services;

public static class QueryOrderingMapper<TEntity, TOrdering>
    where TEntity : class
    where TOrdering : Enum
{
    public static IEnumerable<IQueryOrdering> MapQueryOrderings(IEnumerable<TOrdering>? orderings)
    {
        if (orderings == null || !orderings.Any())
        {
            return new List<IQueryOrdering>
            {
                new QueryOrdering<TEntity> { OrderingPropertyName = "CreatedAt", Descending = true },
            };
        }

        var queryOrderings = new List<IQueryOrdering>();

        foreach (var ordering in orderings)
        {
            string property = ordering.ToString() ?? "CreatedAt";
            property = property
                .Replace("Asc", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Desc", "", StringComparison.OrdinalIgnoreCase);

            bool descending = property.EndsWith("Desc", StringComparison.OrdinalIgnoreCase);

            if (typeof(TEntity).GetProperty(property) != null)
            {
                queryOrderings.Add(
                    new QueryOrdering<TEntity> { OrderingPropertyName = property, Descending = descending }
                );
            }
        }

        return queryOrderings;
    }
}
