using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.CompanyConsumersTests
{
    public class CompanyCreatedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<CompanyCreatedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyCreatedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly CompanyCreatedEventElasticSearchConsumer _consumer;

        public CompanyCreatedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyCreatedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyCreatedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new CompanyCreatedEventElasticSearchConsumer(_mockLogger.Object, _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Document_For_Company_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyCreatedEvent()
            {
                CompanyId = 1,
                CompanyName = "Test Company Name",
                EmailAddress = "test@gmail.com",
                Address = "Test Address",
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
                .Setup(x => x.CreateAsync(
                    It.IsAny<ElasticSearchIndex>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            _mockIndexService
                .Verify(x => x.CreateAsync(
                    It.IsAny<ElasticSearchIndex>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}