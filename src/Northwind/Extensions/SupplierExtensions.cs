using Northwind.Shared.Models;
using Entities = Northwind.Data.Entities;

namespace Northwind.Extensions;

public static class SupplierExtensions
{
    public static IQueryable<Supplier> ToModel(this IQueryable<Entities.Supplier> suppliers)
    {
        return suppliers.Select(e => new Supplier
        {
            SupplierId = e.SupplierId,
            CompanyName = e.CompanyName,
            ContactName = e.ContactName,
            ContactTitle = e.ContactTitle,
            Address = e.Address,
            City = e.City,
            Region = e.Region,
            PostalCode = e.PostalCode,
            Country = e.Country,
            Phone = e.Phone,
            Fax = e.Fax,
            HomePage = e.HomePage
        });
    }
}
