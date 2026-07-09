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
    public class CompanyDeletedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyDeletedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyDeletedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CompanyDeletedEventConsumer _consumer;

        public CompanyDeletedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyDeletedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyDeletedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyDeletedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Delete_Search_Vector_Document_For_Company_When_Event_Received()
        {
            //Arrange
            var document = TestDataSeeder.CreateCompanyDocument();
            await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyDeletedEvent()
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
            var deletedDocument = await _context.SearchVectorDocuments
                .FirstOrDefaultAsync(s => s.EntityId == expectedEvent.CompanyId && s.EntityType == SearchEntityType.Company);

            deletedDocument.ShouldBeNull(); 

            var remainingDocuments = await _context.SearchVectorDocuments.ToListAsync();
            remainingDocuments.ShouldBeEmpty();

        }
        
        [Fact]
        public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyDeletedEvent()
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
            var deletedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            deletedDocument.ShouldBeNull();
        }
    }
}