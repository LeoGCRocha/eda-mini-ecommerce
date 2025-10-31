var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();
    // .WithHostPort(5450)
    // .WithDataVolume()
    // .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("edamicrodb");
    
builder.AddProject<Projects.EdaMicroEcommerce_Api>("edamicroecommerce-api")
    .WithReference(database);

builder.Build().Run();