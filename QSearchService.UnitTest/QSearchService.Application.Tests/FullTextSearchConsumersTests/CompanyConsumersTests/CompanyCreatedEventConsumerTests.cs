using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyConsumers;
using QSearchService.Domain.Enums;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.CompanyConsumersTests
{
    public class CompanyCreatedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyCreatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyCreatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CompanyCreatedEventConsumer _consumer;

        public CompanyCreatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyCreatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyCreatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyCreatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Vector_Document_For_Company_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyCreatedEvent()
            {
                CompanyId = 1,
                CompanyName = "Test Company Name",
                EmailAddress = "test@gmail.com",
                Address = "Test Address",
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
            savedDocument.EntityId.ShouldBe(expectedEvent.CompanyId);
            savedDocument.EntityType.ShouldBe(SearchEntityType.Company);
            savedDocument.Title.ShouldBe(expectedEvent.CompanyName);
            savedDocument.Subtitle.ShouldBe(
                $"{expectedEvent.PhoneNumber}, {expectedEvent.EmailAddress} {expectedEvent.Address}");
        }
    }
}