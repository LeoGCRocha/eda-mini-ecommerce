using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Catalog.Infra;

public class CatalogContextFactory : IDesignTimeDbContextFactory<CatalogContext>
{
    public CatalogContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<CatalogContext>();
        
        var connectionString = configuration.GetConnectionString("EdaMicroDb") 
                               ?? throw new ArgumentException("Parameter connection strign is required");

        builder.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new CatalogContext(builder.Options);
    }
}