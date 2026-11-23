var builder = DistributedApplication.CreateBuilder(args);

builder.AddDotnetProject("catalogapi", "../EshopClaude.Catalog.Api/EshopClaude.Catalog.Api.csproj")
    .WithHttpHealthCheck("/health");

builder.AddDotnetProject("basketapi", "../EshopClaude.Basket.Api/EshopClaude.Basket.Api.csproj")
    .WithHttpHealthCheck("/health");

builder.AddDotnetProject("orderingapi", "../EshopClaude.Ordering.Api/EshopClaude.Ordering.Api.csproj")
    .WithHttpHealthCheck("/health");

builder.AddDotnetProject("webfrontend", "../EshopClaude.Web/EshopClaude.Web.csproj")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

builder.Build().Run();
