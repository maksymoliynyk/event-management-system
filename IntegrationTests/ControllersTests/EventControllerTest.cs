using System.Net.Http;
using System.Threading.Tasks;

using Domain.Commands;

using Microsoft.AspNetCore.Mvc.Testing;

using Faker;
using Shouldly;

using System;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace IntegrationTests.ControllersTests
{
    public class EventControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EventControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        [Fact]
        public async Task CreateEventShouldReturnCreated()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            CreateEventCommand command = new()
            {
                Title = Lorem.Sentence(),
                Description = Lorem.Paragraph(),
                Date = DateTime.Now.AddDays(1),
                Duration = 3600,
                Location = Lorem.Sentence(),
                OwnerEmail = Internet.Email()
            };
            // Act
            StringContent content = new(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("event", content);
            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);


        }
    }
}