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

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.
    CustomerConsumersTests
{
    public class CustomerCreatedEventConsumerTests
    {
        private readonly Mock<ILogger<CustomerCreatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CustomerCreatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CustomerCreatedEventConsumer _consumer;

        public CustomerCreatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CustomerCreatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CustomerCreatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CustomerCreatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Vector_Document_For_Customer_When_Event_Received()
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

            //Act
            await _consumer.Consume(_mockContext.Object);

            //Assert
            var savedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            savedDocument.ShouldNotBeNull();
            savedDocument.EntityId.ShouldBe(expectedEvent.CustomerId);
            savedDocument.EntityType.ShouldBe(SearchEntityType.Customer);
            savedDocument.Title.ShouldBe($"{expectedEvent.FirstName} {expectedEvent.LastName}");
            savedDocument.Subtitle.ShouldBe(expectedEvent.PhoneNumber);
        }
    }
}