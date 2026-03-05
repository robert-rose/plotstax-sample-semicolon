var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Semicolon_Main>("semicolon-main");

builder.Build().Run();
