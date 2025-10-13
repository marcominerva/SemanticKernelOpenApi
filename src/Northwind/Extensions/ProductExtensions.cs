using Northwind.Shared.Models;
using Entities = Northwind.Data.Entities;

namespace Northwind.Extensions;

public static class ProductExtensions
{
    public static IQueryable<Product> ToModel(this IQueryable<Entities.Product> products)
    {
        return products.Select(e => new Product
        {
            ProductId = e.ProductId,
            ProductName = e.ProductName,
            Supplier = e.SupplierId.HasValue ? new()
            {
                SupplierId = e.Supplier!.SupplierId,
                CompanyName = e.Supplier.CompanyName,
                ContactName = e.Supplier.ContactName,
                ContactTitle = e.Supplier.ContactTitle,
                Address = e.Supplier.Address,
                City = e.Supplier.City,
                Region = e.Supplier.Region,
                PostalCode = e.Supplier.PostalCode,
                Country = e.Supplier.Country,
                Phone = e.Supplier.Phone,
                Fax = e.Supplier.Fax,
                HomePage = e.Supplier.HomePage
            } : null,
            Category = e.CategoryId.HasValue ? new()
            {
                CategoryId = e.Category!.CategoryId,
                CategoryName = e.Category.CategoryName,
                Description = e.Category.Description
            } : null,
            QuantityPerUnit = e.QuantityPerUnit,
            UnitPrice = e.UnitPrice,
            UnitsInStock = e.UnitsInStock,
            UnitsOnOrder = e.UnitsOnOrder,
            ReorderLevel = e.ReorderLevel,
            Discontinued = e.Discontinued
        });
    }
}
