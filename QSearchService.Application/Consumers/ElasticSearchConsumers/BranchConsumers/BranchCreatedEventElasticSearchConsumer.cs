using BranchService.Contracts.Events.BranchEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.BranchConsumers;

public class BranchCreatedEventElasticSearchConsumer : IConsumer<BranchCreatedEvent>
{
    private readonly ILogger<BranchCreatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public BranchCreatedEventElasticSearchConsumer(ILogger<BranchCreatedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<BranchCreatedEvent> context)
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
            SearchText =
                $"{request.BranchName} {request.PhoneNumber} {request.EmailAddress} {request.Address} {request.City}"
        };

        await _indexService.CreateAsync(doc, context.CancellationToken);

        _logger.LogInformation("Search document created successfully with Id {Id}", request.BranchId);
    }
}