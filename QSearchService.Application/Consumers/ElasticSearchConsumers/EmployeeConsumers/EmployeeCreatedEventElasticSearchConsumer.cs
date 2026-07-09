using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;

public class EmployeeCreatedEventElasticSearchConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly ILogger<EmployeeCreatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public EmployeeCreatedEventElasticSearchConsumer(ILogger<EmployeeCreatedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType} in Elasticsearch",
            SearchEntityType.Employee);


        var doc = new ElasticSearchIndex
        {
            EntityId = request.EmployeeId,
            EntityType = SearchEntityType.Employee,
            Title = $"{request.FirstName} {request.LastName}",
            Subtitle = $"{request.PhoneNumber} {request.Position}",
            SearchText = $"{request.FirstName} {request.LastName} {request.PhoneNumber} {request.Position}"
        };

        await _indexService.CreateAsync(doc, context.CancellationToken);

        _logger.LogInformation("Search document created successfully with Id {Id}", request.EmployeeId);
    }
}