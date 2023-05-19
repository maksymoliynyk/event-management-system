using Domain.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.RepositoryTests.Helpers
{
    internal static class TestContext
    {
        internal static EventManagementContext GetEventManagementContext()
        {
            return new EventManagementContext(new DbContextOptionsBuilder<EventManagementContext>()
                            .UseNpgsql("Host=localhost;Database=EventManagement;Username=postgres;Password=db_learn")
                            .Options);
        }
    }
}