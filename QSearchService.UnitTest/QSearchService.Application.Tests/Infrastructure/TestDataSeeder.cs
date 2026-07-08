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
}