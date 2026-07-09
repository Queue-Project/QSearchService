using BranchService.Contracts.Events.CompanyServiceEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;

public class CompanyServiceDeletedEventElasticSearchConsumer : IConsumer<CompanyServiceDeletedEvent>
{
    private readonly ILogger<CompanyServiceDeletedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CompanyServiceDeletedEventElasticSearchConsumer(
        ILogger<CompanyServiceDeletedEventElasticSearchConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<CompanyServiceDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        var response = await _client.DeleteAsync<ElasticSearchIndex>(request.CompanyServiceId.ToString(),
            d => d.Index("search-index"));

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to delete document with Id {DocumentId} from Elasticsearch: {response.DebugInformation}",
                request.CompanyServiceId, response.DebugInformation);
            throw new Exception(
                $"Failed to delete document with ID {request.CompanyServiceId} from Elasticsearch: {response.DebugInformation}");
        }

        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}