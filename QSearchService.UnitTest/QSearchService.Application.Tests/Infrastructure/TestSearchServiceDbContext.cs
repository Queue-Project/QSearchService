using Microsoft.EntityFrameworkCore;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;

public class TestSearchServiceDbContext : DbContext, ISearchServiceDbContext
{
    public DbSet<SearchVectorDocument> SearchVectorDocuments { get; set; }

    public TestSearchServiceDbContext(DbContextOptions<TestSearchServiceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SearchVectorDocument>(entity =>
        {
            entity.ToTable("SearchVectorDocuments");
            entity.HasKey(x => x.Id);
            
            entity.Ignore(x => x.SearchVector);
            
            entity.HasIndex(x => new { x.EntityType, x.EntityId }).IsUnique();
            entity.Property(x => x.Title).HasMaxLength(500);
            entity.Property(x => x.Subtitle).HasMaxLength(1000);
            entity.Property(x => x.EntityType).HasMaxLength(100);
        });
    }
}