using Application.Commands.AuthCommands;

using AutoMapper;

using Contracts.Models;

namespace Application.MapperProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        _ = CreateMap<UserDTO, User>();
        _ = CreateMap<RegisterUserCommand, UserDTO>()
                    .ForSourceMember(t => t.Password, opt => opt.DoNotValidate());
    }
}