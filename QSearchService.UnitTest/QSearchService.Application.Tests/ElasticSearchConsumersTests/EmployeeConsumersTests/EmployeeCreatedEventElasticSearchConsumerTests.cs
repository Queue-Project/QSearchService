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
    public class EmployeeCreatedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<EmployeeCreatedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<EmployeeCreatedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly EmployeeCreatedEventElasticSearchConsumer _consumer;

        public EmployeeCreatedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<EmployeeCreatedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<EmployeeCreatedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new EmployeeCreatedEventElasticSearchConsumer(_mockLogger.Object,
                _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Document_For_Employee_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new EmployeeCreatedEvent()
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