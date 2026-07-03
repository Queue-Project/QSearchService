using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace QSearchService.Infrastructure.Persistence.Database;

public class SearchServiceDbContextFactory: IDesignTimeDbContextFactory<SearchServiceDbContext>
{
    public SearchServiceDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<SearchServiceDbContext>();
        
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../QSearchService.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        optionBuilder.UseNpgsql(connectionString);
        return new SearchServiceDbContext(optionBuilder.Options);
    }
}