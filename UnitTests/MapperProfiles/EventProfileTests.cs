using System;

using Application.MapperProfiles;

using AutoMapper;

using Domain.Aggregates.Events;

namespace UnitTests.MapperProfiles
{
    public class EventProfileTests
    {
        private readonly IMapper _mapper;

        public EventProfileTests()
        {
            MapperConfiguration configuration = new(cfg => cfg.AddProfile<EventProfile>());

            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void EventProfileMapsOwnerEmailCorrectly()
        {
            // Arrange
            EventDTO eventDto = new()
            {
                Id = "event1",
                Title = "Event 1",
                Description = "Description of Event 1",
                Date = DateTime.Now,
                Duration = TimeSpan.FromHours(2),
                Location = "Location of Event 1",
                OwnerId = "owner1",
                Owner = new UserDTO
                {
                    Id = "owner1",
                    Email = "owner@example.com"
                },
                Status = 0,
                IsPublic = true
            };

            // Act
            Event eventModel = _mapper.Map<Event>(eventDto);

            // Assert
            Assert.Equal(eventDto.Id, eventModel.Id);
            Assert.Equal(eventDto.Title, eventModel.Title);
            Assert.Equal(eventDto.Description, eventModel.Description);
            Assert.Equal(eventDto.Date, eventModel.Date);
            Assert.Equal(eventDto.Duration, eventModel.Duration);
            Assert.Equal(eventDto.Location, eventModel.Location);
            Assert.Equal(eventDto.Status, (int)eventModel.Status);
            Assert.Equal(eventDto.IsPublic, eventModel.IsPublic);
            Assert.Equal(eventDto.Owner.Email, eventModel.OwnerEmail);
        }
    }
}