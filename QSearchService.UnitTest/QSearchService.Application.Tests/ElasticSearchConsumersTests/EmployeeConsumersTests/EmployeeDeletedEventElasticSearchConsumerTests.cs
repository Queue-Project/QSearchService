using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;
using QSearchService.Application.Interfaces;
using QUserService.Contracts;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.ElasticSearchConsumersTests.
    EmployeeConsumersTests
{
    public class EmployeeDeletedEventElasticSearchConsumerTests
    {
        private readonly Mock<ILogger<EmployeeDeletedEventElasticSearchConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<EmployeeDeletedEvent>> _mockContext;
        private readonly Mock<IElasticSearchIndexService> _mockIndexService;
        private readonly EmployeeDeletedEventElasticSearchConsumer _consumer;

        public EmployeeDeletedEventElasticSearchConsumerTests()
        {
            _mockLogger = new Mock<ILogger<EmployeeDeletedEventElasticSearchConsumer>>();
            _mockContext = new Mock<ConsumeContext<EmployeeDeletedEvent>>();
            _mockIndexService = new Mock<IElasticSearchIndexService>();
            _consumer = new EmployeeDeletedEventElasticSearchConsumer(_mockLogger.Object,
                _mockIndexService.Object);
        }


        [Fact]
        public async Task Consume_Should_Delete_Search_Document_For_Employee_When_Event_Received()
        {
            //Arrange


            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new EmployeeDeletedEvent()
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
            _mockIndexService.Setup(s => s.DeleteAsync(expectedEvent.EmployeeId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert

            _mockIndexService.Verify(s => s.DeleteAsync(expectedEvent.EmployeeId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}