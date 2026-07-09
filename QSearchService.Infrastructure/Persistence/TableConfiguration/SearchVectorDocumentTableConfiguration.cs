using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QSearchService.Domain.Models;

namespace QSearchService.Infrastructure.Persistence.TableConfiguration;

public class SearchVectorDocumentTableConfiguration: IEntityTypeConfiguration<SearchVectorDocument>
{
    public void Configure(EntityTypeBuilder<SearchVectorDocument> builder)
    {
        builder.ToTable("SearchVectorDocuments");

        builder.HasKey(x => x.Id);
        
        builder.HasGeneratedTsVectorColumn(
                x => x.SearchVector,
                "simple",
                x => new
                {
                    x.Title,
                    x.Subtitle
                })
            .HasIndex(x => x.SearchVector)
            .HasMethod("GIN");
        
        builder.HasIndex(x => new { x.EntityType, x.EntityId })
            .IsUnique();


        builder.Property(x => x.Title)
            .HasMaxLength(500);

        builder.Property(x => x.Subtitle)
            .HasMaxLength(1000);

        builder.Property(x => x.EntityType)
            .HasMaxLength(100);
    }
}