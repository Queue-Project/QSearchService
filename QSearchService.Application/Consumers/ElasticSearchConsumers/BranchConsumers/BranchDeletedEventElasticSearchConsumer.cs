using BranchService.Contracts.Events.BranchEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.BranchConsumers;

public class BranchDeletedEventElasticSearchConsumer : IConsumer<BranchDeletedEvent>
{
    private readonly ILogger<BranchDeletedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public BranchDeletedEventElasticSearchConsumer(ILogger<BranchDeletedEventElasticSearchConsumer> logger,
        ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<BranchDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        var response = await _client.DeleteAsync<ElasticSearchIndex>(request.BranchId.ToString(),
            d => d.Index("search-index"));

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to delete document with Id {DocumentId} from Elasticsearch: {response.DebugInformation}",
                request.BranchId, response.DebugInformation);
            throw new Exception(
                $"Failed to delete document with ID {request.BranchId} from Elasticsearch: {response.DebugInformation}");
        }

        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}