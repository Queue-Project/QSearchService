using Elastic.Clients.Elasticsearch;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Services;

public class ElasticSearchIndexService : IElasticSearchIndexService
{
    private readonly ElasticsearchClient _client;

    public ElasticSearchIndexService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task CreateAsync(
        ElasticSearchIndex document,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.IndexAsync(
            document,
            i => i.Index("search-index")
                .Id(document.EntityId.ToString()),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new Exception("Failed to create search document.");
        }
    }

    public async Task UpdateAsync(
        ElasticSearchIndex document,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.UpdateAsync<ElasticSearchIndex, ElasticSearchIndex>(
            "search-index",
            document.EntityId.ToString(),
            u => u.Doc(document).DocAsUpsert(true),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new Exception("Failed to update search document.");
        }
    }

    public async Task DeleteAsync(
        int entityId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteAsync<ElasticSearchIndex>(
            entityId.ToString(),
            d => d.Index("search-index"),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new Exception("Failed to delete search document.");
        }
    }
}