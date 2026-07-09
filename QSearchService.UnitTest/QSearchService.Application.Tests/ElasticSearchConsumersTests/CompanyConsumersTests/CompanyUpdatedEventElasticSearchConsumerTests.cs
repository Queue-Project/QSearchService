using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;


namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.CompanyConsumersTests
{
    public class CompanyUpdatedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<CompanyUpdatedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyUpdatedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly CompanyUpdatedEventElasticSearchConsumer _consumer;

        public CompanyUpdatedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyUpdatedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyUpdatedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new CompanyUpdatedEventElasticSearchConsumer(_mockLogger.Object, _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Updated_Search_Document_For_Company_When_Event_Received()
        {
            //Arrange


            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyUpdatedEvent()
            {
                CompanyId = 1,
                CompanyName = "Update Company Name",
                EmailAddress = "update@gmail.com",
                Address = "Update Address",
                PhoneNumber = "+992923324252",
                OccuredAt = occuredAt,
                AuditData = new AuditData
                {
                    PerformedByUserId = 1,
                    PerformedByUserName = "systemAdmin",
                    Changes = new List<AuditEventLogDetails>()
                }
            };

            _mockContext.Setup(s => s.Message).Returns(expectedEvent);
            _mockContext.Setup(s => s.CancellationToken).Returns(CancellationToken.None);
            _mockIndexService
                .Setup(x => x.UpdateAsync(
                    It.IsAny<ElasticSearchIndex>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            _mockIndexService
                .Verify(x => x.UpdateAsync(
                    It.IsAny<ElasticSearchIndex>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}