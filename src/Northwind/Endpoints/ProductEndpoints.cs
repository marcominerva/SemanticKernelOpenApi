using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.OpenApi;
using Northwind.Data;
using Northwind.Extensions;
using Northwind.Shared.Models;

namespace Northwind.Endpoints;

public class ProductEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/products").WithTags("Products");

        // GET /products
        group.MapGet("/", async (NorthwindContext db, ClaimsPrincipal user, ILogger<ProductEndpoints> logger, CancellationToken cancellationToken) =>
        {
            logger.LogInformation("User {user} is retrieving all products", user.Identity?.Name ?? "anonymous");

            var items = await db.Products
                .OrderBy(p => p.ProductName)
                .ToModel()
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(items);
        })
        .RequireAuthorization()
        .WithSummary("Gets all products. Each product contains also all the information about its supplier and its category")
        .WithGroupName("ai")
        .WithName("Products_GetAll");

        // GET /products/{id}
        group.MapGet("/{id:int}", async Task<Results<Ok<Product>, NotFound>> (int id, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var entity = await db.Products
                .Where(p => p.ProductId == id).ToModel().FirstOrDefaultAsync(cancellationToken);

            return entity is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(entity);
        })
        .ProducesDefaultProblem(StatusCodes.Status404NotFound)
        .WithName("Products_GetById");

        // POST /products
        group.MapPost("/", async (NewProduct input, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var entity = new Data.Entities.Product
            {
                ProductName = input.ProductName,
                SupplierId = input.SupplierId,
                CategoryId = input.CategoryId,
                QuantityPerUnit = input.QuantityPerUnit,
                UnitPrice = input.UnitPrice,
                UnitsInStock = input.UnitsInStock,
                UnitsOnOrder = input.UnitsOnOrder,
                ReorderLevel = input.ReorderLevel,
                Discontinued = input.Discontinued
            };

            db.Products.Add(entity);
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.CreatedAtRoute("Products_GetById", new { id = entity.ProductId });
        })
        .WithValidation<NewProduct>()
        .WithName("Products_Create");

        // PUT /products/{id}
        group.MapPut("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, NewProduct input, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var entity = await db.Products.AsTracking().FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);

            if (entity is null)
            {
                return TypedResults.NotFound();
            }

            entity.CategoryId = input.CategoryId;
            entity.ProductName = input.ProductName;
            entity.SupplierId = input.SupplierId;
            entity.QuantityPerUnit = input.QuantityPerUnit;
            entity.UnitPrice = input.UnitPrice;
            entity.UnitsInStock = input.UnitsInStock;
            entity.UnitsOnOrder = input.UnitsOnOrder;
            entity.ReorderLevel = input.ReorderLevel;
            entity.Discontinued = input.Discontinued;

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return TypedResults.NoContent();
        })
        .WithValidation<NewProduct>()
        .ProducesDefaultProblem(StatusCodes.Status404NotFound)
        .WithName("Products_Update");

        // DELETE /products/{id}
        group.MapDelete("/{id:int}", async (int id, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            await db.Products.Where(p => p.ProductId == id)
                .ExecuteDeleteAsync(cancellationToken);

            return TypedResults.NoContent();
        })
        .WithName("Products_Delete");
    }
}
