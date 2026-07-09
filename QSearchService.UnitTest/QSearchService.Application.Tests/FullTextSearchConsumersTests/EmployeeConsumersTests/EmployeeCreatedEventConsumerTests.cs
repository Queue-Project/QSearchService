using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.FullTextSearchConsumers.EmployeeConsumers;
using QSearchService.Domain.Enums;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using QUserService.Contracts;
using QUserService.Contracts.Events.EmployeeEvent;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.
    EmployeeConsumersTests
{
    public class EmployeeCreatedEventConsumerTests
    {
        private readonly Mock<ILogger<EmployeeCreatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<EmployeeCreatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly EmployeeCreatedEventConsumer _consumer;

        public EmployeeCreatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<EmployeeCreatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<EmployeeCreatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new EmployeeCreatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Vector_Document_For_Employee_When_Event_Received()
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

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            var savedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            savedDocument.ShouldNotBeNull();
            savedDocument.EntityId.ShouldBe(expectedEvent.EmployeeId);
            savedDocument.EntityType.ShouldBe(SearchEntityType.Employee);
            savedDocument.Title.ShouldBe($"{expectedEvent.FirstName} {expectedEvent.LastName}");
            savedDocument.Subtitle.ShouldBe($"{expectedEvent.PhoneNumber} {expectedEvent.Position}");
        }
    }
}