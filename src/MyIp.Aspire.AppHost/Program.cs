var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MyIp>("myip");

builder.Build().Run();
