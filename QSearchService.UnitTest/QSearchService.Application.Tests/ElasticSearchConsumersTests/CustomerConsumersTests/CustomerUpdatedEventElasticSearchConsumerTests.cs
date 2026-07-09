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
    public class CustomerUpdatedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<CustomerUpdatedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CustomerUpdatedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly CustomerUpdatedEventElasticSearchConsumer _consumer;

        public CustomerUpdatedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CustomerUpdatedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<CustomerUpdatedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new CustomerUpdatedEventElasticSearchConsumer(_mockLogger.Object,
                _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Updated_Search_Vector_Document_For_Customer_When_Event_Received()
        {
            //Arrange

            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CustomerUpdatedEvent()
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