using Application.Commands.Auth;
using Application.MapperProfiles;

using AutoMapper;

using Contracts.Models;

namespace UnitTests.MapperProfiles
{
    public class UserProfileTests
    {
        private readonly IMapper _mapper;

        public UserProfileTests()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());

            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void UserProfileMapsUserDTOToUserCorrectly()
        {
            // Arrange
            UserDTO userDto = new UserDTO
            {
                Id = "user1",
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            User userModel = _mapper.Map<User>(userDto);

            // Assert
            Assert.Equal(userDto.Id, userModel.Id);
            Assert.Equal(userDto.Email, userModel.Email);
            Assert.Equal(userDto.FirstName, userModel.FirstName);
            Assert.Equal(userDto.LastName, userModel.LastName);
            Assert.Equal(userDto.FullName, userModel.FullName);
        }

        [Fact]
        public void UserProfileMapsRegisterUserCommandToUserDTOCorrectly()
        {
            // Arrange
            RegisterUserCommand registerUserCommand = new RegisterUserCommand
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "password123"
            };

            // Act
            UserDTO userDto = _mapper.Map<UserDTO>(registerUserCommand);

            // Assert
            Assert.Equal(registerUserCommand.Email, userDto.Email);
            Assert.Equal(registerUserCommand.FirstName, userDto.FirstName);
            Assert.Equal(registerUserCommand.LastName, userDto.LastName);
            Assert.Equal(registerUserCommand.Username, userDto.UserName);
            Assert.Null(userDto.PasswordHash);
        }
    }
}
