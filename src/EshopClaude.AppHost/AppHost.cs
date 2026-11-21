var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddDotnetProject("apiservice", "../EshopClaude.ApiService/EshopClaude.ApiService.csproj")
    .WithHttpHealthCheck("/health");

builder.AddDotnetProject("webfrontend", "../EshopClaude.Web/EshopClaude.Web.csproj")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
