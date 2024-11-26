using Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.DbContexts
{
    public class EventManagementContextTests
    {
        [Fact]
        public void EventManagementContextWithoutOptionsConstructsSuccessfully()
        {
            // Arrange & Act
            EventManagementContext dbContext = new();

            // Assert
            Assert.NotNull(dbContext);
        }

        [Fact]
        public void EventManagementContextWithOptionsConstructsSuccessfully()
        {
            // Arrange
            DbContextOptions<EventManagementContext> options = new DbContextOptionsBuilder<EventManagementContext>()
                .Options;

            // Act
            EventManagementContext dbContext = new(options);

            // Assert
            Assert.NotNull(dbContext);
        }

        [Fact]
        public void EventsDbSetsIsNotNull()
        {
            // Arrange & Act
            EventManagementContext dbContext = new();

            // Assert
            Assert.NotNull(dbContext.Events);
            Assert.NotNull(dbContext.RSPVs);
        }

        [Fact]
        public void OnConfiguringIsConfigured()
        {
            // Arrange
            DbContextOptionsBuilder<EventManagementContext> optionsBuilder = new DbContextOptionsBuilder<EventManagementContext>();
            EventManagementContext dbContext = new EventManagementContext();
            _ = dbContext.InvokePrivateMethod<object>("OnConfiguring", optionsBuilder);

            // Assert
            Assert.True(optionsBuilder.IsConfigured);
        }
    }
}