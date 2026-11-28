var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(pgAdmin =>
    {
        // pgAdmin binds to IPv6 only by default; Aspire's proxy and health check use IPv4.
        pgAdmin.WithEnvironment("PGADMIN_LISTEN_ADDRESS", "0.0.0.0");

        // Aspire hides pgAdmin by default; show it in the dashboard's resource list.
        foreach (var hidden in pgAdmin.Resource.Annotations.OfType<HiddenAnnotation>().ToList())
        {
            pgAdmin.Resource.Annotations.Remove(hidden);
        }
    });

var catalogdb = postgres.AddDatabase("catalogdb");
var orderingdb = postgres.AddDatabase("orderingdb");

builder.AddRedis("redis");

builder.AddDotnetProject("catalogapi", "../EshopClaude.Catalog.Api/EshopClaude.Catalog.Api.csproj")
    .WithReference(catalogdb)
    .WaitFor(catalogdb)
    .WithHttpHealthCheck("/health");

builder.AddDotnetProject("basketapi", "../EshopClaude.Basket.Api/EshopClaude.Basket.Api.csproj")
    .WithHttpHealthCheck("/health");

builder.AddDotnetProject("orderingapi", "../EshopClaude.Ordering.Api/EshopClaude.Ordering.Api.csproj")
    .WithReference(orderingdb)
    .WaitFor(orderingdb)
    .WithHttpHealthCheck("/health");

builder.AddDotnetProject("webfrontend", "../EshopClaude.Web/EshopClaude.Web.csproj")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

builder.Build().Run();
