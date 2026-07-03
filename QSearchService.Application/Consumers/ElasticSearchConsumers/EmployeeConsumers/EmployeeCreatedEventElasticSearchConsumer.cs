using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;

public class EmployeeCreatedEventElasticSearchConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly ILogger<EmployeeCreatedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public EmployeeCreatedEventElasticSearchConsumer(ILogger<EmployeeCreatedEventElasticSearchConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
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

        var response = await _client.IndexAsync(doc, i =>
            i.Index("search-index").Id(request.EmployeeId.ToString()), context.CancellationToken);

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to index employee {EmployeeId}: {Reason}",
                request.EmployeeId,
                response.ElasticsearchServerError?.Error.Reason);

            throw new Exception(
                $"Failed to index employee {request.EmployeeId}");
        }

        _logger.LogInformation("Search document created successfully with Id {Id}", request.EmployeeId);
    }
}