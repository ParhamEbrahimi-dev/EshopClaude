var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapGet("/", () => "Catalog API is running.");

app.MapDefaultEndpoints();

app.Run();
