using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;

namespace Billing.Infras;

public class CatalogContextFactory : IDesignTimeDbContextFactory<BillingContext>
{
    public BillingContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<BillingContext>();
        
        var connectionString = configuration.GetConnectionString("EdaMicroDb") 
                               ?? throw new ArgumentException("Parameter: ConnectionString required");

        builder.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new BillingContext(builder.Options);
    }
}