var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.IdentityService>("identity-service")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.API>("ecommerceapp-api")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.StoreFront>("storefront")
    .WithExternalHttpEndpoints();

builder.Build().Run();
