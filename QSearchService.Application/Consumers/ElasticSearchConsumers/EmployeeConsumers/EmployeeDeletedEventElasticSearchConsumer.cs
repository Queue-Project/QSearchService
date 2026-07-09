using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;

public class EmployeeDeletedEventElasticSearchConsumer : IConsumer<EmployeeDeletedEvent>
{
    private readonly ILogger<EmployeeDeletedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;


    public EmployeeDeletedEventElasticSearchConsumer(ILogger<EmployeeDeletedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        await _indexService.DeleteAsync(request.EmployeeId, context.CancellationToken);

        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}