using Microsoft.EntityFrameworkCore;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Interfaces;

public interface ISearchServiceDbContext
{
    DbSet<SearchVectorDocument> SearchVectorDocuments { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
}