using BranchService.Contracts.Events;
using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.BranchConsumers;
using QSearchService.Application.Consumers.FullTextSearchConsumers.BranchConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;


namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.BranchConsumers;

public class BranchDeletedEventElasticSearchConsumerTests
{
    private readonly Mock<ILogger<BranchDeletedEventElasticSearchConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<BranchDeletedEvent>> _mockContext;
    private readonly Mock<IElasticSearchIndexService> _mockIndexService;
    private readonly BranchDeletedEventElasticSearchConsumer _consumer;

    public BranchDeletedEventElasticSearchConsumerTests()
    {
        _mockLogger = new Mock<ILogger<BranchDeletedEventElasticSearchConsumer>>();
        _mockContext = new Mock<ConsumeContext<BranchDeletedEvent>>();
        _mockIndexService = new Mock<IElasticSearchIndexService>();
        _consumer = new BranchDeletedEventElasticSearchConsumer(_mockLogger.Object, _mockIndexService.Object);
    }

    [Fact]
    public async Task Consume_Should_Delete_Search_Document_For_Branch_When_Event_Received()
    {
        // Arrange
        var occuredAt = DateTime.UtcNow;

        var expectedEvent = new BranchDeletedEvent
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
            .Setup(x => x.DeleteAsync(
                expectedEvent.BranchId,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockContext.Object);

        // Assert
        _mockIndexService.Verify(x => x.DeleteAsync(
                expectedEvent.BranchId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}