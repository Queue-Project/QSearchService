using BranchService.Contracts.Events;
using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.BranchConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;


namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.BranchConsumers;

public class BranchCreatedEventElasticSearchConsumerTests
{
    private readonly Mock<ILogger<BranchCreatedEventElasticSearchConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<BranchCreatedEvent>> _mockContext;
    private readonly Mock<IElasticSearchIndexService> _mockIndexService;
    private readonly BranchCreatedEventElasticSearchConsumer _consumer;

    public BranchCreatedEventElasticSearchConsumerTests()
    {
        _mockLogger = new Mock<ILogger<BranchCreatedEventElasticSearchConsumer>>();
        _mockContext = new Mock<ConsumeContext<BranchCreatedEvent>>();
        _mockIndexService = new Mock<IElasticSearchIndexService>();
        _consumer = new BranchCreatedEventElasticSearchConsumer(_mockLogger.Object, _mockIndexService.Object);
    }

    [Fact]
    public async Task Consume_Should_Create_Search_Document_For_Branch_When_Event_Received()
    {
        // Arrange
        var occuredAt = DateTime.UtcNow;

        var expectedEvent = new BranchCreatedEvent
        {
            CompanyId = 1,
            BranchId = 1,
            BranchName = "Test Branch Name",
            EmailAddress = "test@gmail.com",
            Address = "Test Address",
            City = "Test City",
            IsActive = true,
            PhoneNumber = "+992923324252",
            OccuredAt = occuredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "systemAdmin",
                Changes = new List<AuditEventLogDetails>()
            }
        };

        _mockContext.Setup(x => x.Message).Returns(expectedEvent);
        _mockContext.Setup(x => x.CancellationToken).Returns(CancellationToken.None);

        _mockIndexService
            .Setup(x => x.CreateAsync(
                It.IsAny<ElasticSearchIndex>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockContext.Object);

        // Assert
        _mockIndexService.Verify(x => x.CreateAsync(
                It.Is<ElasticSearchIndex>(doc =>
                    doc.EntityId == expectedEvent.BranchId &&
                    doc.EntityType == SearchEntityType.Branch &&
                    doc.Title == expectedEvent.BranchName &&
                    doc.Subtitle == $"{expectedEvent.PhoneNumber} {expectedEvent.EmailAddress}" &&
                    doc.SearchText ==
                    $"{expectedEvent.BranchName} {expectedEvent.PhoneNumber} {expectedEvent.EmailAddress} {expectedEvent.Address} {expectedEvent.City}"
                ),
                CancellationToken.None),
            Times.Once);
    }
}