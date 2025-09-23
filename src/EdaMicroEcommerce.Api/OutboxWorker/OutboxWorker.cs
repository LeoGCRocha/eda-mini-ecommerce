using EdaMicroEcommerce.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EdaMicroEcommerce.Api.OutboxWorker;

public class OutboxWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private const int MaxBatchSize = 1000;

    public OutboxWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EdaContext>();
            
            var integrationEvents = await dbContext.OutboxIntegrationEvents.Where(p => p.ProcessedAtUtc == null)
                .OrderBy(p => p.CreatedAtUtc).Take(MaxBatchSize)
                .ToListAsync(cancellationToken: stoppingToken);
            
            // TODO: Message processing handler
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}