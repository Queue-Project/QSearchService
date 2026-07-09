using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyServiceConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.
    CompanyServiceConsumersTests
{
    public class CompanyServiceCreatedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<CompanyServiceCreatedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyServiceCreatedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly CompanyServiceCreatedEventElasticSearchConsumer _consumer;

        public CompanyServiceCreatedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyServiceCreatedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyServiceCreatedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new CompanyServiceCreatedEventElasticSearchConsumer(_mockLogger.Object,
                _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Document_For_Company_Service_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceCreatedEvent()
            {
                CompanyId = 1,
                CompanyServiceId = 1,
                ServiceName = "Test Service Name",
                ServiceDescription = "Test Description",
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
            _mockIndexService.Setup(s => s.CreateAsync(It.IsAny<ElasticSearchIndex>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            _mockIndexService.Verify(s => s.CreateAsync(It.IsAny<ElasticSearchIndex>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}