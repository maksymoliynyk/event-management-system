using AutoMapper;

using Domain.Commands.AuthCommands;
using Domain.MapperProfiles;
using Domain.Models.Database;

using Faker;

using Shouldly;

namespace UnitTests.MappingTests
{
    public class UserMappingTests
    {
        private readonly IMapper _mapper;

        public UserMappingTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>()).CreateMapper();
        }

        [Fact]
        public void RegisterUserCommandShouldMapToUserDTO()
        {
            // Arrange
            RegisterUserCommand registerUserCommand = new RegisterUserCommand
            {
                Email = Internet.Email(),
                Password = Lorem.GetFirstWord(),
                FirstName = Name.First(),
                LastName = Name.Last(),
                Username = Internet.UserName()
            };

            // Act
            UserDTO userDTO = _mapper.Map<RegisterUserCommand, UserDTO>(registerUserCommand);

            // Assert
            userDTO.Email.ShouldBe(registerUserCommand.Email);
            userDTO.FirstName.ShouldBe(registerUserCommand.FirstName);
            userDTO.LastName.ShouldBe(registerUserCommand.LastName);
            userDTO.UserName.ShouldBe(registerUserCommand.Username);
            userDTO.PasswordHash.ShouldBeNull();
        }
    }
}