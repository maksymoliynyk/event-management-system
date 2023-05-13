using AutoMapper;

using Contracts.Models;

using Domain.Models.Database;

namespace Domain.MapperProfiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            _ = CreateMap<EventDTO, Event>()
                .ForMember(dest => dest.OwnerEmail,
                    src =>
                    {
                        src.PreCondition(t => t.Owner is not null);
                        src.MapFrom(t => t.Owner.Email);
                    }
                );
        }
    }
}