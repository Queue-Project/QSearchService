using QSearchService.Domain.Models;

namespace QSearchService.Application.Interfaces;

public interface IElasticSearchIndexService
{
    Task CreateAsync(
        ElasticSearchIndex document,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ElasticSearchIndex document,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int entityId,
        CancellationToken cancellationToken = default);
}