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
    public class EmployeeDeletedEventConsumerTests
    {
        private readonly Mock<ILogger<EmployeeDeletedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<EmployeeDeletedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly EmployeeDeletedEventConsumer _consumer;

        public EmployeeDeletedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<EmployeeDeletedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<EmployeeDeletedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new EmployeeDeletedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Delete_Search_Vector_Document_For_Employee_When_Event_Received()
        {
            //Arrange
            var document = TestDataSeeder.CreateEmployeeDocument();
            await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
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

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            var deletedDocument = await _context.SearchVectorDocuments
                .FirstOrDefaultAsync(s => s.EntityId == expectedEvent.EmployeeId && s.EntityType == SearchEntityType.Employee);

            deletedDocument.ShouldBeNull(); 

            var remainingDocuments = await _context.SearchVectorDocuments.ToListAsync();
            remainingDocuments.ShouldBeEmpty();

        }
        
        [Fact]
        public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
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

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            var deletedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            deletedDocument.ShouldBeNull();
        }
    }
}