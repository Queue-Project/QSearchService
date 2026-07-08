using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CustomerConsumers;

public class CustomerDeletedEventElasticSearchConsumer : IConsumer<CustomerDeletedEvent>
{
    private readonly ILogger<CustomerDeletedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CustomerDeletedEventElasticSearchConsumer(ILogger<CustomerDeletedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<CustomerDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        await _indexService.DeleteAsync(request.CustomerId, context.CancellationToken);
        

        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}