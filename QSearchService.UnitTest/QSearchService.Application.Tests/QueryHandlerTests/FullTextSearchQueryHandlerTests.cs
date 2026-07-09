using Microsoft.Extensions.Logging;
using Moq;
using QSearchService.Application.UseCases.Queries.FullTextSearch;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.QueryHandlerTests;

public class FullTextSearchQueryHandlerTests
{
    private readonly Mock<ILogger<FullTextSearchQueryHandler>> _mockLogger;
    private readonly TestSearchServiceDbContext _dbContext;
    private readonly FullTextSearchQueryHandler _handler;

    public FullTextSearchQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<FullTextSearchQueryHandler>>();
        _dbContext = TestDbContextFactory.Create();
        _handler = new FullTextSearchQueryHandler(_mockLogger.Object, _dbContext);
    }
    
    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_Search_Term_Is_Null()
    {
        //Arrange

        string searchTerm = "";
        int pageNumber = 1;
        int pageSize = 10;
        var query = new FullTextSearchQuery(searchTerm, pageNumber, pageSize);


        //Act

        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert


        result.PageNumber.ShouldBe(pageNumber);
        result.PageSize.ShouldBe(pageSize);
        result.TotalCount.ShouldBe(0);
        result.HasNextPage.ShouldBe(false);
        result.HasPreviousPage.ShouldBe(false);
        result.TotalPages.ShouldBe(0);
    }
}