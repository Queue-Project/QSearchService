using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QSearchService.Infrastructure.Persistence.Database;

public class SearchServiceDbContextFactory: IDesignTimeDbContextFactory<SearchServiceDbContext>
{
    public SearchServiceDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<SearchServiceDbContext>();
        optionBuilder.UseNpgsql("Host=localhost;Port=5432;Database=QSearchService;Username=postgres;Password=b.sh.3242");
        return new SearchServiceDbContext(optionBuilder.Options);
    }
}