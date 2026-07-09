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

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.CompanyServiceConsumersTests
{
    public class CompanyServiceDeletedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyServiceDeletedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyServiceDeletedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CompanyServiceDeletedEventConsumer _consumer;

        public CompanyServiceDeletedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyServiceDeletedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyServiceDeletedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyServiceDeletedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Delete_Search_Vector_Document_For_Company_Service_When_Event_Received()
        {
            //Arrange
            var document = TestDataSeeder.CreateCompanyServiceDocument();
            await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceDeletedEvent()
            {
                CompanyId = 1,
                CompanyServiceId = 1,
                ServiceName = "Update Company Service Name",
                ServiceDescription = "Update Description",
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
                .FirstOrDefaultAsync(s => s.EntityId == expectedEvent.CompanyServiceId && s.EntityType == SearchEntityType.CompanyService);

            deletedDocument.ShouldBeNull(); 

            var remainingDocuments = await _context.SearchVectorDocuments.ToListAsync();
            remainingDocuments.ShouldBeEmpty();

        }
        
        [Fact]
        public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceDeletedEvent()
            {
                CompanyId = 1,
                CompanyServiceId = 1,
                ServiceName = "Update Company Service Name",
                ServiceDescription = "Update Description",
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