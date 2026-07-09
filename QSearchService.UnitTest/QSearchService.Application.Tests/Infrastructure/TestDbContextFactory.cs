using Microsoft.EntityFrameworkCore;

namespace QSearchService.UnitTest.QSearchService.Application.Tests.Infrastructure;

public static class TestDbContextFactory
{
    public static TestSearchServiceDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestSearchServiceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new TestSearchServiceDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}