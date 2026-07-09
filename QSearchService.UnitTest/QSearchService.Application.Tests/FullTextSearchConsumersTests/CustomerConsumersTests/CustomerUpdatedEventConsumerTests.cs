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
    public class CustomerUpdatedEventConsumerTests
    {
        private readonly Mock<ILogger<CustomerUpdatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CustomerUpdatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CustomerUpdatedEventConsumer _consumer;

        public CustomerUpdatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CustomerUpdatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CustomerUpdatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CustomerUpdatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Updated_Search_Vector_Document_For_Customer_When_Event_Received()
        {
            //Arrange
            var document = TestDataSeeder.CreateCustomerDocument();
            await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CustomerUpdatedEvent()
            {
                CustomerId = 1,
                FirstName = "Update First Name",
                LastName = "Update Last Name",
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
            var updatedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            updatedDocument.ShouldNotBeNull();
            updatedDocument.EntityId.ShouldBe(expectedEvent.CustomerId);
            updatedDocument.EntityType.ShouldBe(SearchEntityType.Customer);
            updatedDocument.Title.ShouldBe($"{expectedEvent.FirstName} {expectedEvent.LastName}");
            updatedDocument.Subtitle.ShouldBe(expectedEvent.PhoneNumber);
        }
        
        [Fact]
        public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CustomerUpdatedEvent()
            {
                CustomerId = 1,
                FirstName = "Update First Name",
                LastName = "Update Last Name",
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
            var updatedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            updatedDocument.ShouldBeNull();
        }
    }
}