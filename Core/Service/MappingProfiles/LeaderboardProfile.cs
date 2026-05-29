using AutoMapper;
using Domain.Entities.Groups;
using Service.MappingProfiles.MediaResolvers;
using Shared.DTOs.LeaderboardModule;

namespace Service.MappingProfiles;

public class LeaderboardProfile : Profile
{
    public LeaderboardProfile()
    {
        CreateMap<GroupScore, LeaderboardGroupDTO>()
            .ForMember(dest => dest.GroupId,
                opt => opt.MapFrom(src => src.GroupId))

            .ForMember(dest => dest.GroupName,
                opt => opt.MapFrom(src => src.Group.GroupName))

            .ForMember(dest => dest.GroupProfilePicture,
                opt => opt.MapFrom(src => src.Group))

            .ForMember(dest => dest.TotalPoints,
                opt => opt.MapFrom(src => src.TotalPoints))

            .ForMember(dest => dest.Rank,
                opt => opt.MapFrom(src => src.Rank));
    }
}