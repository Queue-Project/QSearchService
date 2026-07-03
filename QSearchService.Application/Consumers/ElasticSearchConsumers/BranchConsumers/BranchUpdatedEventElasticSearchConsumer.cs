using BranchService.Contracts.Events.BranchEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.BranchConsumers;

public class BranchUpdatedEventElasticSearchConsumer : IConsumer<BranchUpdatedEvent>
{
    private readonly ILogger<BranchUpdatedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public BranchUpdatedEventElasticSearchConsumer(ILogger<BranchUpdatedEventElasticSearchConsumer> logger,
        ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<BranchUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType} in Elasticsearch",
            SearchEntityType.Branch);


        var doc = new ElasticSearchIndex
        {
            EntityId = request.BranchId,
            EntityType = SearchEntityType.Branch,
            Title = request.BranchName,
            Subtitle = $"{request.PhoneNumber} {request.EmailAddress}",
            SearchText = $"{request.BranchName} {request.PhoneNumber} {request.EmailAddress} {request.Address} {request.City}"
        };

        var response = await _client.IndexAsync(doc, i =>
            i.Index("search-index").Id(request.BranchId.ToString()), context.CancellationToken);

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to index branch {BranchId}: {Reason}",
                request.BranchId,
                response.ElasticsearchServerError?.Error.Reason);

            throw new Exception(
                $"Failed to index branch {request.BranchId}");
        }

        _logger.LogInformation("Search document created successfully with Id {Id}", request.BranchId);
    }
}