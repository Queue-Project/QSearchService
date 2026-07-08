using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyServiceConsumers;
using QSearchService.Domain.Enums;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.
    CompanyServiceConsumersTests
{
    public class CompanyServiceCreatedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyServiceCreatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyServiceCreatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CompanyServiceCreatedEventConsumer _consumer;

        public CompanyServiceCreatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyServiceCreatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyServiceCreatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyServiceCreatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Search_Vector_Document_For_Company_Service_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceCreatedEvent()
            {
                CompanyId = 1,
                CompanyServiceId = 1,
                ServiceName = "Test Service Name",
                ServiceDescription = "Test Description",
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
            savedDocument.EntityId.ShouldBe(expectedEvent.CompanyServiceId);
            savedDocument.EntityType.ShouldBe(SearchEntityType.CompanyService);
            savedDocument.Title.ShouldBe(expectedEvent.ServiceName);
            savedDocument.Subtitle.ShouldBe(expectedEvent.ServiceDescription);
        }
    }
}