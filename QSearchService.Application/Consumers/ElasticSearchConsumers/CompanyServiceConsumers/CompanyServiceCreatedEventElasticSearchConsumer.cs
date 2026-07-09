using BranchService.Contracts.Events.CompanyServiceEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;

public class CompanyServiceCreatedEventElasticSearchConsumer : IConsumer<CompanyServiceCreatedEvent>
{
    private readonly ILogger<CompanyServiceCreatedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CompanyServiceCreatedEventElasticSearchConsumer(ILogger<CompanyServiceCreatedEventElasticSearchConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<CompanyServiceCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType} in Elasticsearch",
            SearchEntityType.CompanyService);


        var doc = new ElasticSearchIndex
        {
            EntityId = request.CompanyServiceId,
            EntityType = SearchEntityType.CompanyService,
            Title = request.ServiceName,
            Subtitle = request.ServiceDescription,
            SearchText = $"{request.ServiceName} {request.ServiceDescription} "
        };

        var response = await _client.IndexAsync(doc, i =>
            i.Index("search-index").Id(request.CompanyServiceId.ToString()), context.CancellationToken);

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to index company service {ServiceId}: {Reason}",
                request.CompanyServiceId,
                response.ElasticsearchServerError?.Error.Reason);

            throw new Exception(
                $"Failed to index company service {request.CompanyServiceId}");
        }

        _logger.LogInformation("Search document created successfully with Id {Id}", request.CompanyServiceId);
    }
}