var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapGet("/", () => "Basket API is running.");

app.MapDefaultEndpoints();

app.Run();
