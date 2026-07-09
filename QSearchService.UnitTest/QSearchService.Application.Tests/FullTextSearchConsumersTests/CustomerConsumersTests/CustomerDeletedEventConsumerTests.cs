using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CustomerConsumers;
using QSearchService.Domain.Enums;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using QUserService.Contracts;
using QUserService.Contracts.Events.CustomerEvent;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.CustomerConsumersTests
{
    public class CustomerDeletedEventConsumerTests
    {
        private readonly Mock<ILogger<CustomerDeletedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CustomerDeletedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CustomerDeletedEventConsumer _consumer;

        public CustomerDeletedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CustomerDeletedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CustomerDeletedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CustomerDeletedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Delete_Search_Vector_Document_For_Customer_When_Event_Received()
        {
            //Arrange
            var document = TestDataSeeder.CreateCustomerDocument();
            await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CustomerDeletedEvent()
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

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            var deletedDocument = await _context.SearchVectorDocuments
                .FirstOrDefaultAsync(s => s.EntityId == expectedEvent.CustomerId && s.EntityType == SearchEntityType.Customer);

            deletedDocument.ShouldBeNull(); 

            var remainingDocuments = await _context.SearchVectorDocuments.ToListAsync();
            remainingDocuments.ShouldBeEmpty();

        }
        
        [Fact]
        public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CustomerDeletedEvent()
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

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            var deletedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            deletedDocument.ShouldBeNull();
        }
    }
}