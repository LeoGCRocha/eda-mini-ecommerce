using Microsoft.EntityFrameworkCore;
using EdaMicroEcommerce.Infra.Persistence;
using EdaMicroEcommerce.Application.IntegrationEvents;

namespace EdaMicroEcommerce.Api.OutboxWorker;

public class OutboxWorker(IServiceProvider serviceProvider, ILogger<OutboxWorker> logger)
    : BackgroundService
{
    private const int MaxBatchSize = 1000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(OutboxWorker)} started execution.");
        while (!stoppingToken.IsCancellationRequested)
        {
            // TODO: Adicionar SCHEMA REGISTRY
            // TODO: Make some tests
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EdaContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>();
            
            var integrationEvents = await dbContext.OutboxIntegrationEvents.Where(p => p.ProcessedAtUtc == null && !p.IsDeadLetter)
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
                    // TODO: pass cancellation token
                    await publisher.PublishOnTopicAsync(@event);
                    @event.SetProcessedAtToNow();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Message cannot be published");
                    @event.UpdateRetryCount();
                    
                    if (@event.RetryCount > 5)
                        @event.MarkAsDead();
                }
            });
            
            await dbContext.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}