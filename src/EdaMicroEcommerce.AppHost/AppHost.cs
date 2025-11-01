var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("Postgres")
    .WithHostPort(5450)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var kafka = builder.AddKafka("KafkaMessageBroker")
    .WithKafkaUI(kafkaUi => kafkaUi.WithHostPort(9100))
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("EdaMicroDb");

builder.AddProject<Projects.EdaMicroEcommerce_Api>("edamicroecommerce-api")
    .WithReference(database)
    .WithReference(kafka);

builder.AddProject<Projects.Catalog_InventoryWorker>("edamicroecommerce-catalog-worker")
    .WithReference(database)
    .WithReference(kafka);

builder.AddProject<Projects.Orders_SagaOrchestrator>("edamicroecommerce-saga-orchestrator")
    .WithReference(database)
    .WithReference(kafka);

builder.Build().Run();