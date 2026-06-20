using CleanArch.AppHost;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CleanArch_Api>("clean-arch-api")
    .WithHttpHealthCheck("/health")
    .WithScalar();

await builder.Build().RunAsync();
