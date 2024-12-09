using Application.MapperProfiles;

using AutoMapper;

using Domain.Aggregates.Events;

namespace UnitTests.MapperProfiles
{
    public class RSVPProfileTests
    {
        private readonly IMapper _mapper;

        public RSVPProfileTests()
        {
            MapperConfiguration configuration = new(cfg => cfg.AddProfile<RSVPProfile>());

            _mapper = configuration.CreateMapper();
        }
        [Fact]
        public void RSVPProfileMapsItemsCorrectly()
        {
            // Arrange
            RSVPDTO rsvpDTO = new()
            {
                Id = "rsvp123",
                UserId = "user123",
                User = new UserDTO
                {
                    Id = "user123",
                    Email = "user123@example.com"
                },
                EventId = "event123",
                Event = new EventDTO
                {
                    Id = "event123",
                    Title = "New Event"
                },
                Status = 0
            };

            // Act
            RSVP rsvpModel = _mapper.Map<RSVP>(rsvpDTO);
            // Assert
            Assert.Equal(rsvpDTO.Id, rsvpModel.Id);
            Assert.Equal(rsvpDTO.Event.Title, rsvpModel.EventTitle);
            Assert.Equal(rsvpDTO.User.Email, rsvpModel.UserEmail);
            Assert.Equal(rsvpDTO.Status, (int)rsvpModel.Status);
        }
    }
}