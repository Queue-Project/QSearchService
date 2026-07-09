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

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.EmployeeConsumersTests
{
    public class EmployeeUpdatedEventConsumerTests
    {
        private readonly Mock<ILogger<EmployeeUpdatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<EmployeeUpdatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly EmployeeUpdatedEventConsumer _consumer;

        public EmployeeUpdatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<EmployeeUpdatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<EmployeeUpdatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new EmployeeUpdatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Updated_Search_Vector_Document_For_Employee_When_Event_Received()
        {
            //Arrange
            var document = TestDataSeeder.CreateEmployeeDocument();
            await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new EmployeeUpdatedEvent()
            {
                EmployeeId = 1,
                FirstName = "Update First Name",
                LastName = "Update Last Name",
                PhoneNumber = "+992922223242",
                Position = "Update Position",
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
            var updatedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            updatedDocument.ShouldNotBeNull();
            updatedDocument.EntityId.ShouldBe(expectedEvent.EmployeeId);
            updatedDocument.EntityType.ShouldBe(SearchEntityType.Employee);
            updatedDocument.Title.ShouldBe($"{expectedEvent.FirstName} {expectedEvent.LastName}");
            updatedDocument.Subtitle.ShouldBe($"{expectedEvent.PhoneNumber} {expectedEvent.Position}");
        }
        
        [Fact]
        public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new EmployeeUpdatedEvent()
            {
                EmployeeId = 1,
                FirstName = "Update First Name",
                LastName = "Update Last Name",
                PhoneNumber = "+992922223242",
                Position = "Update Position",
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
            var updatedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            updatedDocument.ShouldBeNull();
        }
    }
}