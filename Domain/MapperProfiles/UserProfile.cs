using AutoMapper;

using Contracts.Models;

using Domain.Commands.AuthCommands;
using Domain.Models.Database;

namespace Domain.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            _ = CreateMap<UserDTO, User>();
            _ = CreateMap<RegisterUserCommand, UserDTO>()
                        .ForSourceMember(t => t.Password, opt => opt.DoNotValidate());
        }
    }
}