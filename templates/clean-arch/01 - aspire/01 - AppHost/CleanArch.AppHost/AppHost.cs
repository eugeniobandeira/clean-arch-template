var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.CleanArch_Api>("cleanarch-api");

builder.Build().Run();
