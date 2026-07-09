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
    public class CompanyUpdatedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyUpdatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyUpdatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CompanyUpdatedEventConsumer _consumer;

        public CompanyUpdatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyUpdatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyUpdatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyUpdatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Updated_Search_Vector_Document_For_Company_When_Event_Received()
        {
            //Arrange
            var document = TestDataSeeder.CreateCompanyDocument();
            await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyUpdatedEvent()
            {
                CompanyId = 1,
                CompanyName = "Update Company Name",
                EmailAddress = "update@gmail.com",
                Address = "Update Address",
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
            var updatedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            updatedDocument.ShouldNotBeNull();
            updatedDocument.EntityId.ShouldBe(expectedEvent.CompanyId);
            updatedDocument.EntityType.ShouldBe(SearchEntityType.Company);
            updatedDocument.Title.ShouldBe(expectedEvent.CompanyName);
            updatedDocument.Subtitle.ShouldBe(
                $"{expectedEvent.PhoneNumber}, {expectedEvent.EmailAddress} {expectedEvent.Address}");
        }
        
        [Fact]
        public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyUpdatedEvent()
            {
                CompanyId = 1,
                CompanyName = "Update Company Name",
                EmailAddress = "update@gmail.com",
                Address = "Update Address",
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
            var updatedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            updatedDocument.ShouldBeNull();
        }
    }
}