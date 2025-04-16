var builder = DistributedApplication.CreateBuilder(args);

// Add redis cache
//var redis = builder.AddRedis("redis", 6379)
//    .WithDataVolume("redis-data"); ;

//// Add postgresql database
//var db = builder.AddPostgres("postgres")
//    .WithEnvironment("POSTGRES_USER", "user")
//    .WithEnvironment("POSTGRES_PASSWORD", "password")
//    .WithDataVolume("postgres-data");

//// Add azure blog storage
//var blobStorage = builder.AddAzureStorage("storage")
//    .RunAsEmulator(config => config.WithDataVolume("azure-blob-data"))
//    .AddBlobs("blobs");

builder.AddProject<Projects.IdentityService>("identity-service");

builder.AddProject<Projects.API>("ecommerceapp-api");
    //.WithReference(redis)
    //.WithReference(db)
    //.WithReference(blobStorage);

builder.AddProject<Projects.StoreFront>("storefront");
    //.WithReference(redis)
    //.WithReference(db)
    //.WithReference(blobStorage);

builder.Build().Run();
