using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.
    CompanyServiceConsumersTests
{
    public class CompanyServiceUpdatedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<CompanyServiceUpdatedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyServiceUpdatedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly CompanyServiceUpdatedEventElasticSearchConsumer _consumer;

        public CompanyServiceUpdatedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyServiceUpdatedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyServiceUpdatedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new CompanyServiceUpdatedEventElasticSearchConsumer(_mockLogger.Object,
                _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Updated_Search_Vector_Document_For_Company_Service_When_Event_Received()
        {
            //Arrange

            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceUpdatedEvent()
            {
                CompanyId = 1,
                CompanyServiceId = 1,
                ServiceName = "Update Company Service Name",
                ServiceDescription = "Update Description",
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
            _mockIndexService.Setup(s => s.UpdateAsync(It.IsAny<ElasticSearchIndex>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert

            _mockIndexService.Verify(s => s.UpdateAsync(It.IsAny<ElasticSearchIndex>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}