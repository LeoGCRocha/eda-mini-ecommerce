var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithHostPort(5450)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var kafka = builder.AddKafka("kafka")
    .WithKafkaUI(kafkaUi => kafkaUi.WithHostPort(9100));

var database = postgres.AddDatabase("edamicrodb");

builder.AddProject<Projects.EdaMicroEcommerce_Api>("edamicroecommerce-api")
    .WithReference(database)
    .WithReference(kafka);

builder.Build().Run();