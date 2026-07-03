using BranchService.Contracts.Events.CompanyEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;

public class CompanyDeletedEventElasticSearchConsumer : IConsumer<CompanyDeletedEvent>
{
    private readonly ILogger<CompanyDeletedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CompanyDeletedEventElasticSearchConsumer(ILogger<CompanyDeletedEventElasticSearchConsumer> logger,
        ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<CompanyDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        var response = await _client.DeleteAsync<ElasticSearchIndex>(request.CompanyId.ToString(),
            d => d.Index("search-index"));

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to delete document with Id {DocumentId} from Elasticsearch: {response.DebugInformation}",
                request.CompanyId, response.DebugInformation);
            throw new Exception(
                $"Failed to delete document with ID {request.CompanyId} from Elasticsearch: {response.DebugInformation}");
        }

        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}