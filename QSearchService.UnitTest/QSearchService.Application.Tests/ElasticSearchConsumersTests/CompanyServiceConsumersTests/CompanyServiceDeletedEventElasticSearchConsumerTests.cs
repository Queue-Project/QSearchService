using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;
using QSearchService.Application.Interfaces;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.
    CompanyServiceConsumersTests
{
    public class CompanyServiceDeletedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<CompanyServiceDeletedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyServiceDeletedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly CompanyServiceDeletedEventElasticSearchConsumer _consumer;

        public CompanyServiceDeletedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyServiceDeletedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyServiceDeletedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new CompanyServiceDeletedEventElasticSearchConsumer(_mockLogger.Object,
                _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Delete_Search_Document_For_Company_Service_When_Event_Received()
        {
            //Arrange


            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceDeletedEvent()
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
            _mockIndexService.Setup(s => s.DeleteAsync(expectedEvent.CompanyServiceId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert

            _mockIndexService.Verify(s => s.DeleteAsync(expectedEvent.CompanyServiceId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}