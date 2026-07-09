using QSearchService.Application.Responses;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;

public static class TestDataSeeder
{
    public static SearchVectorDocument CreateBranchDocument()
    {
        return new SearchVectorDocument
        {
            Id = 1,
            EntityId = 1,
            EntityType = SearchEntityType.Branch,
            Title = "Test Branch Name",
            Subtitle = $"+992923324252 test@gmail.com Test Address Test City",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static SearchVectorDocument CreateCompanyDocument()
    {
        return new SearchVectorDocument
        {
            Id = 1,
            EntityId = 1,
            EntityType = SearchEntityType.Company,
            Title = "Test Company Name",
            Subtitle = $"+992923324252 test@gmail.com Test Address",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }


    public static SearchVectorDocument CreateCompanyServiceDocument()
    {
        return new SearchVectorDocument
        {
            Id = 1,
            EntityId = 1,
            EntityType = SearchEntityType.CompanyService,
            Title = "Test Company Service Name",
            Subtitle = $"Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static SearchVectorDocument CreateCustomerDocument()
    {
        return new SearchVectorDocument
        {
            Id = 1,
            EntityId = 1,
            EntityType = SearchEntityType.Customer,
            Title = "Test First Name Test Last Name",
            Subtitle = $"+992923324252",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static SearchVectorDocument CreateEmployeeDocument()
    {
        return new SearchVectorDocument
        {
            Id = 1,
            EntityId = 1,
            EntityType = SearchEntityType.Employee,
            Title = "Test First Name Test Last Name",
            Subtitle = $"+992923324252 Test Position",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static PagedResponse<SearchItem> GetSearchResult()
    {
        return new PagedResponse<SearchItem>
        {
            Items = new List<SearchItem>
            {
                new SearchItem
                {
                    EntityId = 1,
                    EntityType = SearchEntityType.Customer,
                    Title = "Test First Name Test Last Name",
                    Subtitle = "+992923324252",
                    Rank = 0.9444444
                },
                new SearchItem
                {
                    EntityId = 2,
                    EntityType = SearchEntityType.Employee,
                    Title = "Test First Name Test Last Name",
                    Subtitle = "+992923324252 Test Position",
                    Rank = 0.8999999

                },
                new SearchItem
                {
                    EntityId = 3,
                    EntityType = SearchEntityType.Customer,
                    Title = "Test First Name2 Test Last Name2",
                    Subtitle = "+992923324251",
                    Rank = 0.8222222
                },
                new SearchItem
                {
                    EntityId = 4,
                    EntityType = SearchEntityType.Employee,
                    Title = "Test First Name4 Test Last Name4",
                    Subtitle = "+992923324254 Test Position4",
                    Rank = 0.788888
                }
            },
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 4
        };
    }
}