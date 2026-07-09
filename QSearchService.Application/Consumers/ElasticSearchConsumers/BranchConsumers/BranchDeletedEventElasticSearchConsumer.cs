using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.BranchConsumers;

public class BranchDeletedEventElasticSearchConsumer : IConsumer<BranchDeletedEvent>
{
    private readonly ILogger<BranchDeletedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public BranchDeletedEventElasticSearchConsumer(
        ILogger<BranchDeletedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<BranchDeletedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Deleting search document from Elasticsearch");

        await _indexService.DeleteAsync(
            request.BranchId,
            context.CancellationToken);

        _logger.LogInformation(
            "Search document deleted successfully with Id {Id}",
            request.BranchId);
    }
}