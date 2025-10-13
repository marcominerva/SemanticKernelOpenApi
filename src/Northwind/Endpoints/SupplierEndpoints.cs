using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalHelpers.OpenApi;
using Northwind.Data;
using Northwind.Extensions;
using Northwind.Shared.Models;

namespace Northwind.Endpoints;

public class SupplierEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/suppliers").WithTags("Suppliers");

        // GET /suppliers
        group.MapGet("/", async (NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var items = await db.Suppliers
                .OrderBy(s => s.CompanyName)
                .ToModel()
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(items);
        })
        .WithName("Suppliers_GetAll");

        // GET /suppliers/{id}
        group.MapGet("/{id:int}", async Task<Results<Ok<Supplier>, NotFound>> (int id, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var entity = await db.Suppliers
                .Where(s => s.SupplierId == id)
                .ToModel()
                .FirstOrDefaultAsync(cancellationToken);

            return entity is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(entity);
        })
        .ProducesDefaultProblem(StatusCodes.Status404NotFound)
        .WithName("Suppliers_GetById");
    }
}
