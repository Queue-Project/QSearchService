using BranchService.Contracts.Events;
using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.FullTextSearchConsumers.BranchConsumers;
using QSearchService.Domain.Enums;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.BranchConsumersTests
{
    public class BranchCreatedEventConsumerTests
    {
        private readonly Mock<ILogger<BranchCreatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<BranchCreatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly BranchCreatedEventConsumer _consumer;

        public BranchCreatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<BranchCreatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<BranchCreatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new BranchCreatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Vector_Document_For_Branch_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new BranchCreatedEvent()
            {
                CompanyId = 1,
                BranchId = 1,
                BranchName = "Test Branch Name",
                EmailAddress = "test@gmail.com",
                Address = "Test Address",
                City = "Test City",
                IsActive = true,
                PhoneNumber = "+992923324252",
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
            savedDocument.EntityId.ShouldBe(expectedEvent.BranchId);
            savedDocument.EntityType.ShouldBe(SearchEntityType.Branch);
            savedDocument.Title.ShouldBe(expectedEvent.BranchName);
            savedDocument.Subtitle.ShouldBe(
                $"{expectedEvent.PhoneNumber} {expectedEvent.EmailAddress} {expectedEvent.Address} {expectedEvent.City}");
        }
    }
}