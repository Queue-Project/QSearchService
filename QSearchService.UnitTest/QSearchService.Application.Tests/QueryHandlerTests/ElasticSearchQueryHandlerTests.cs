using Moq;
using QSearchService.Application.Interfaces;
using QSearchService.Application.UseCases.Queries.ElasticSearch;
using QSearchService.Domain.Enums;
using QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;
using Shouldly;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.QueryHandlerTests;

public class ElasticSearchQueryHandlerTests
{
    private readonly Mock<IElasticSearchService> _mockElasticSearchService;
    private readonly ElasticSearchQueryHandler _handler;

    public ElasticSearchQueryHandlerTests()
    {
        _mockElasticSearchService = new Mock<IElasticSearchService>();
        _handler = new ElasticSearchQueryHandler(_mockElasticSearchService.Object);
    }

    [Fact]
    public async Task Handler_Should_Return_Search_Result_When_Search_Term_Is_Not_Null()
    {
        //Arrange

        string searchTerm = "test";
        int pageNumber = 1;
        int pageSize = 10;
        var query = new ElasticSearchQuery(searchTerm, pageNumber, pageSize);

        var expectedResponse = TestDataSeeder.GetSearchResult();

        _mockElasticSearchService
            .Setup(s => s.SearchAsync(searchTerm, pageNumber, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        //Act

        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert


        result.PageNumber.ShouldBe(pageNumber);
        result.PageSize.ShouldBe(pageSize);
        result.TotalCount.ShouldBe(4);
        result.HasNextPage.ShouldBe(false);
        result.HasPreviousPage.ShouldBe(false);
        result.TotalPages.ShouldBe(1);

        var firstItem = result.Items.FirstOrDefault();
        firstItem!.EntityId.ShouldBe(1);
        firstItem.EntityType.ShouldBe(SearchEntityType.Customer);
        firstItem.Title.ShouldBe("Test First Name Test Last Name");
        firstItem.Subtitle.ShouldBe("+992923324252");
        firstItem.Rank.ShouldBe(0.9444444);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_Search_Term_Is_Null()
    {
        //Arrange

        string searchTerm = "";
        int pageNumber = 1;
        int pageSize = 10;
        var query = new ElasticSearchQuery(searchTerm, pageNumber, pageSize);


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