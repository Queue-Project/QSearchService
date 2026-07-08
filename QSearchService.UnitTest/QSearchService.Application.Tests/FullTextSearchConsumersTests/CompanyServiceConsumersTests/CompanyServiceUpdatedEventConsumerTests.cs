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
    public class CompanyServiceUpdatedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyServiceUpdatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyServiceUpdatedEvent>> _mockContext;
        private readonly TestSearchServiceDbContext _context;
        private readonly CompanyServiceUpdatedEventConsumer _consumer;

        public CompanyServiceUpdatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyServiceUpdatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyServiceUpdatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyServiceUpdatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Updated_Search_Vector_Document_For_Company_Service_When_Event_Received()
        {
            //Arrange
            var document = TestDataSeeder.CreateCompanyServiceDocument();
            await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceUpdatedEvent()
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
            var updatedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            updatedDocument.ShouldNotBeNull();
            updatedDocument.EntityId.ShouldBe(expectedEvent.CompanyId);
            updatedDocument.EntityType.ShouldBe(SearchEntityType.CompanyService);
            updatedDocument.Title.ShouldBe(expectedEvent.ServiceName);
            updatedDocument.Subtitle.ShouldBe(expectedEvent.ServiceDescription);
        }
        
        [Fact]
        public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceUpdatedEvent()
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
            var updatedDocument = await _context.SearchVectorDocuments.FirstOrDefaultAsync();

            updatedDocument.ShouldBeNull();
        }
    }
}