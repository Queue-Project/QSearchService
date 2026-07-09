using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CustomerConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;
using QUserService.Contracts;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.
    CustomerConsumersTests
{
    public class CustomerCreatedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<CustomerCreatedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CustomerCreatedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly CustomerCreatedEventElasticSearchConsumer _consumer;

        public CustomerCreatedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CustomerCreatedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<CustomerCreatedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new CustomerCreatedEventElasticSearchConsumer(_mockLogger.Object,
                _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Document_For_Customer_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CustomerCreatedEvent()
            {
                CustomerId = 1,
                FirstName = "Test First Name",
                LastName = "Test Last Name",
                PhoneNumber = "+992922223242",
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