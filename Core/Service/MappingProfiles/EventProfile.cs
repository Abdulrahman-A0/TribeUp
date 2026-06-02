using AutoMapper;
using Domain.Entities.Engagement;
using Shared.DTOs.EventModule;
using Shared.Enums;

namespace Service.MappingProfiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<CreateEventDTO, Event>();

            CreateMap<Event, EventResponseDTO>()
                .ForMember(
                    dest => dest.GoingCount,
                    opt => opt.MapFrom(src =>
                        src.Participants.Count(p =>
                            p.Status == EventResponseStatus.Going)))
                .ForMember(
                    dest => dest.InterestedCount,
                    opt => opt.MapFrom(src =>
                        src.Participants.Count(p =>
                            p.Status == EventResponseStatus.Interested)))
                .ForMember(
                    dest => dest.NotGoingCount,
                    opt => opt.MapFrom(src =>
                        src.Participants.Count(p =>
                            p.Status == EventResponseStatus.NotGoing)));
        }
    }
}