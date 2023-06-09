using AutoMapper;

using Contracts.Models;

using Domain.Models.Database;

namespace Domain.MapperProfiles
{
    public class RSVPProfile : Profile
    {
        public RSVPProfile()
        {
            _ = CreateMap<RSVPDTO, RSVP>()
                .ForMember(dest => dest.EventTitle,
                    src =>
                    {
                        src.PreCondition(t => t.Event is not null);
                        src.MapFrom(t => t.Event.Title);
                    }
                )
                .ForMember(dest => dest.UserEmail,
                    src =>
                    {
                        src.PreCondition(t => t.User is not null);
                        src.MapFrom(t => t.User.Email);
                    }
                );
        }
    }
}