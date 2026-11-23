var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapGet("/", () => "Ordering API is running.");

app.MapDefaultEndpoints();

app.Run();
