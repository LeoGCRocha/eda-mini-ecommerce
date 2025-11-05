using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Orders.Infra;

public class OrderContextFactory : IDesignTimeDbContextFactory<OrderContext>
{
    public OrderContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<OrderContext>();
        
        var connectionString = configuration.GetConnectionString("EdaMicroDb") 
                               ?? throw new ArgumentException("Parameter connection strign is required");

        builder.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new OrderContext(builder.Options);
    }
}