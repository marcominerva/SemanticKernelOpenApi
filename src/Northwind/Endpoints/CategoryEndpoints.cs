using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.OpenApi;
using Northwind.Data;
using Northwind.Extensions;
using Northwind.Shared.Models;

namespace Northwind.Endpoints;

public class CategoryEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/categories").WithTags("Categories");

        // GET /categories
        group.MapGet("/", async (NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var items = await db.Categories
                .OrderBy(c => c.CategoryName)
                .ToModel()
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(items);
        })
        .WithGroupName("ai")
        .WithName("Categories_GetAll");

        // GET /categories/{id}
        group.MapGet("/{id:int}", async Task<Results<Ok<Category>, NotFound>> (int id, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var entity = await db.Categories
                .Where(c => c.CategoryId == id)
                .ToModel()
                .FirstOrDefaultAsync(cancellationToken);

            return entity is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(entity);
        })
        .ProducesDefaultProblem(StatusCodes.Status404NotFound)
        .WithName("Categories_GetById");

        // POST /categories
        group.MapPost("/", async (NewCategory input, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var entity = new Data.Entities.Category
            {
                CategoryName = input.CategoryName,
                Description = input.Description
            };

            db.Categories.Add(entity);
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.CreatedAtRoute("Categories_GetById", new { id = entity.CategoryId });
        })
        .WithGroupName("ai")
        .WithValidation<NewCategory>()
        .WithName("Categories_Create");

        // PUT /categories/{id}
        group.MapPut("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, NewCategory input, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            var entity = await db.Categories.AsTracking().FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);

            if (entity is null)
            {
                return TypedResults.NotFound();
            }

            entity.CategoryName = input.CategoryName;
            entity.Description = input.Description;

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return TypedResults.NoContent();
        })
        .WithGroupName("ai")
        .WithValidation<NewCategory>()
        .ProducesDefaultProblem(StatusCodes.Status404NotFound)
        .WithName("Categories_Update");

        // DELETE /categories/{id}
        group.MapDelete("/{id:int}", async (int id, NorthwindContext db, CancellationToken cancellationToken) =>
        {
            await db.Categories.Where(c => c.CategoryId == id)
                .ExecuteDeleteAsync(cancellationToken);

            return TypedResults.NoContent();
        })
        .WithName("Categories_Delete");
    }
}
