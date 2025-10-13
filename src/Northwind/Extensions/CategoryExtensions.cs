using Northwind.Shared.Models;
using Entities = Northwind.Data.Entities;

namespace Northwind.Extensions;

public static class CategoryExtensions
{
    public static IQueryable<Category> ToModel(this IQueryable<Entities.Category> categories)
    {
        return categories.Select(e => new Category
        {
            CategoryId = e.CategoryId,
            CategoryName = e.CategoryName,
            Description = e.Description
        });
    }
}
