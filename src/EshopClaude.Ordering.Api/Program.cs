using EshopClaude.Ordering.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<OrderingDbContext>("orderingdb");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "Ordering API is running.");

app.MapDefaultEndpoints();

app.Run();
