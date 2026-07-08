using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.BranchConsumers;

public class BranchUpdatedEventElasticSearchConsumer : IConsumer<BranchUpdatedEvent>
{
    private readonly ILogger<BranchUpdatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public BranchUpdatedEventElasticSearchConsumer(
        ILogger<BranchUpdatedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<BranchUpdatedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Updating search document in Elasticsearch");

        var document = new ElasticSearchIndex
        {
            EntityId = request.BranchId,
            EntityType = SearchEntityType.Branch,
            Title = request.BranchName,
            Subtitle = $"{request.PhoneNumber} {request.EmailAddress}",
            SearchText =
                $"{request.BranchName} {request.PhoneNumber} {request.EmailAddress} {request.Address} {request.City}"
        };

        await _indexService.UpdateAsync(
            document,
            context.CancellationToken);

        _logger.LogInformation(
            "Search document updated successfully with Id {Id}",
            request.BranchId);
    }
}