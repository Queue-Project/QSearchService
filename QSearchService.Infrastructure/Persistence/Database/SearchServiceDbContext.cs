using Microsoft.EntityFrameworkCore;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;
using QSearchService.Infrastructure.Persistence.TableConfiguration;

namespace QSearchService.Infrastructure.Persistence.Database;

public class SearchServiceDbContext: DbContext, ISearchServiceDbContext
{

    public DbSet<SearchVectorDocument> SearchVectorDocuments { get; set; }

    public SearchServiceDbContext(DbContextOptions<SearchServiceDbContext> options): base(options)
    {
        
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SearchVectorDocumentTableConfiguration).Assembly);
    }
}