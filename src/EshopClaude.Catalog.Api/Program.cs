using EshopClaude.Catalog.Api;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.EnsureCreated();
    CatalogSeed.Seed(db);
}

app.MapGet("/", () => "Catalog API is running.");

app.MapGet("/products", async (CatalogDbContext db) =>
    await db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync());

app.MapGet("/products/{id:int}", async Task<Results<Ok<Product>, NotFound>> (int id, CatalogDbContext db) =>
    await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id) is { } product
        ? TypedResults.Ok(product)
        : TypedResults.NotFound());

app.MapDefaultEndpoints();

app.Run();
