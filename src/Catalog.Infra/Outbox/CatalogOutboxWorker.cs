using MediatR;
using Catalog.Application.IntegrationEvents;
using Catalog.Application.IntegrationEvents.Products.ProductDeactivated;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Catalog.Infra.Outbox;

public class CatalogOutboxWorker(IServiceProvider serviceProvider, ILogger<CatalogOutboxWorker> logger)
    : BackgroundService
{
    private const int MaxBatchSize = 1000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(CatalogOutboxWorker)} started execution.");
        while (!stoppingToken.IsCancellationRequested)
        {
            // <WARNING> Temos um possível problema aqui no OUTBOX que é se for modificado o nome da classe o outbox ficaria perdido
            // para encontrar a respectiva implementação

            // TODO: Adicionar SCHEMA REGISTRY
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CatalogContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            // <WARNING> Talvez isso aqui não deveria estar externalizado via EF e sim acesso via query diretamente.
            var integrationEvents = await dbContext.OutboxIntegrationEvents
                .Where(p => p.ProcessedAtUtc == null && !p.IsDeadLetter)
                .OrderBy(p => p.CreatedAtUtc).Take(MaxBatchSize)
                .ToListAsync(cancellationToken: stoppingToken);

            int parallelism = Environment.ProcessorCount * 2; // io bound operation
            await Parallel.ForEachAsync(integrationEvents, new ParallelOptions
            {
                MaxDegreeOfParallelism = parallelism
            }, async (@event, ct) =>
            {
                // at least once guarantee
                try
                {
                    switch (@event.Type)
                    {
                        case EventType.ProductDeactivated:
                            await mediator.Send(
                                new ProductDeactivatedIntegration(EventType.ProductDeactivated, @event.Payload), ct);
                            break;
                        default:
                            throw new ArgumentException("Tipo inesperado para EventType");
                    }

                    @event.SetProcessedAtToNow();
                }
                catch (Exception ex)
                {
                    // <WARNING> Aqui temos um problema na forma que o OUTBOX foi estruturado onde não existe um contexto
                    // especifico do erro, ou seja, dificultando tratamentos mais especificos pra uma mensagem.
                    logger.LogError(ex, "Message cannot be published");
                    @event.UpdateRetryCount();

                    if (@event.RetryCount > 5)
                        @event.MarkAsDead();
                }
            });

            await dbContext.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}