using CleanArch.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CleanArch_Api>("clean-arch-api")
    .WithHttpHealthCheck("/health")
    .WithScalar();

await builder.Build().RunAsync();
