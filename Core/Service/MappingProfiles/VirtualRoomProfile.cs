using AutoMapper;
using Domain.Entities.VirtualRooms;
using Shared.DTOs.VirtualRoomModule;

namespace Service.MappingProfiles
{
    public class VirtualRoomProfile : Profile
    {
        public VirtualRoomProfile()
        {
            CreateMap<RoomParticipant, ParticipantDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName ?? "Unknown"))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.Avatar ?? "/models/default_avatar.glb"));
        }
    }
}
