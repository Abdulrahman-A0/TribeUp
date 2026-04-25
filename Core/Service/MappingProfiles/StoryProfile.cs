using AutoMapper;
using Domain.Entities.Stories;
using Service.MappingProfiles.MediaResolvers;
using Shared.DTOs.StoriesModule;

namespace Service.MappingProfiles
{
    public class StoryProfile : Profile
    {
        public StoryProfile()
        {

            CreateMap<CreateStoryDTO, Story>()
                .ForMember(dest => dest.MediaURL, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(24)));


            CreateMap<Story, StoryResponseDTO>()
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatorUserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.MediaURL, opt => opt.MapFrom<StoryMediaResolver>())
                .ForMember(dest => dest.IsViewedByCurrentUser, opt => opt.Ignore())
                .ForMember(dest => dest.ViewsCount, opt => opt.Ignore());
        }
    }
}
