using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;
using QUserService.Contracts;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.
    EmployeeConsumersTests
{
    public class EmployeeUpdatedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<EmployeeUpdatedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<EmployeeUpdatedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly EmployeeUpdatedEventElasticSearchConsumer _consumer;

        public EmployeeUpdatedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<EmployeeUpdatedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<EmployeeUpdatedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new EmployeeUpdatedEventElasticSearchConsumer(_mockLogger.Object,
                _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Updated_Search_Vector_Document_For_Customer_When_Event_Received()
        {
            //Arrange

            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new EmployeeUpdatedEvent()
            {
                EmployeeId = 1,
                FirstName = "Test First Name",
                LastName = "Test Last Name",
                PhoneNumber = "+992922223242",
                Position = "Test Position",
                OccurredAt = occuredAt,
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