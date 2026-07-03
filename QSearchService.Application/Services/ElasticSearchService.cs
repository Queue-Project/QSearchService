using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using QSearchService.Application.Interfaces;
using QSearchService.Application.Responses;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Services;

public sealed class ElasticSearchService : IElasticSearchService
{
    private readonly ElasticsearchClient _client;

    public ElasticSearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<PagedResponse<SearchItem>> SearchAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var response = await _client.SearchAsync<ElasticSearchIndex>(
            s => s
                .From((pageNumber - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(searchTerm)
                        .Fields(new[]
                        {
                            Infer.Field<ElasticSearchIndex>(x => x.Title),
                            Infer.Field<ElasticSearchIndex>(x => x.Subtitle),
                            Infer.Field<ElasticSearchIndex>(x => x.SearchText)
                        })
                        .Fuzziness(new Fuzziness("AUTO"))
                        .Operator(Operator.Or)
                        .Type(TextQueryType.BestFields))),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new Exception(
                $"Elasticsearch search failed: {response.DebugInformation}");
        }

        return new PagedResponse<SearchItem>
        {
            Items = response.Hits
                .Select(hit => new SearchItem
                {
                    EntityId = hit.Source!.EntityId,
                    EntityType = hit.Source.EntityType,
                    Title = hit.Source.Title,
                    Subtitle = hit.Source.Subtitle,
                    Rank = hit.Score ?? 0
                })
                .ToList(),

            TotalCount = (int)(response.HitsMetadata.Total?.Value1?.Value ?? 0),

            PageNumber = pageNumber,

            PageSize = pageSize
        };
    }
}