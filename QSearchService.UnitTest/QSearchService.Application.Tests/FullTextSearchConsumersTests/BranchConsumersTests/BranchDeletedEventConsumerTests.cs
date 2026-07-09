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

namespace QSearchService.UnitTest.QSearchService.Application.Tests.FullTextSearchConsumersTests.BranchConsumersTests;

public class BranchDeletedEventConsumerTests
{
    private readonly Mock<ILogger<BranchDeletedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<BranchDeletedEvent>> _mockContext;
    private readonly TestSearchServiceDbContext _context;
    private readonly BranchDeletedEventConsumer _consumer;

    public BranchDeletedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<BranchDeletedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<BranchDeletedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new BranchDeletedEventConsumer(_mockLogger.Object, _context);
    }
    
    [Fact]
    public async Task Consume_Should_Delete_Search_Vector_Document_For_Branch_When_Event_Received_And_Document_Exists()
    {
        //Arrange

        var document = TestDataSeeder.CreateBranchDocument();
        await _context.SearchVectorDocuments.AddAsync(document, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);
        
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new BranchDeletedEvent()
        {
            CompanyId = 1,
            BranchId = 1,
            BranchName = "Updated Branch Name",
            EmailAddress = "test@gmail.com",
            Address = "Updated Address",
            City = "Updated City",
            IsActive = true,
            PhoneNumber = "+992923324211",
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
            .FirstOrDefaultAsync(s => s.EntityId == expectedEvent.BranchId && s.EntityType == SearchEntityType.Branch);

        deletedDocument.ShouldBeNull(); 

        var remainingDocuments = await _context.SearchVectorDocuments.ToListAsync();
        remainingDocuments.ShouldBeEmpty();
        
    }
    
    [Fact]
    public async Task Consume_Should_Return_Null_When_Event_Received_And_Document_Not_Exists()
    {
        //Arrange
        
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new BranchDeletedEvent()
        {
            CompanyId = 1,
            BranchId = 1,
            BranchName = "Updated Branch Name",
            EmailAddress = "test@gmail.com",
            Address = "Updated Address",
            City = "Updated City",
            IsActive = true,
            PhoneNumber = "+992923324211",
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