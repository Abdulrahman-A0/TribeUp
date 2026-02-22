using AutoMapper;
using Domain.Entities.Users;
using Service.MappingProfiles.MediaResolvers;
using Shared.DTOs.IdentityModule;
using Shared.DTOs.ProfileModule;

namespace Service.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserProfileDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom<UserProfilePictureResolver, string>(src => src.ProfilePicture!));

            CreateMap<RegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<ApplicationUser, string>().ConvertUsing((src, dest, context) =>
                context.Mapper.Map<UserProfileDTO>(src).ProfilePicture);

            CreateMap<ApplicationUser, UserSummaryDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom<UserProfilePictureResolver, string>(src => src.ProfilePicture!));


            CreateMap<ApplicationUser, ProfileSettingsDTO>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom<UserProfilePictureResolver, string>(src => src.ProfilePicture!))
                .ForMember(dest => dest.CoverPicture, opt => opt.MapFrom<UserCoverPictureResolver<ProfileSettingsDTO>>());


            CreateMap<ApplicationUser, UserProfileHeaderDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom<UserProfilePictureResolver, string>(src => src.ProfilePicture))
                .ForMember(dest => dest.CoverPicture, opt => opt.MapFrom<UserCoverPictureResolver<UserProfileHeaderDTO>>())
                .ForMember(dest => dest.PostsCount, opt => opt.MapFrom(src => src.Posts.Count))
                .ForMember(dest => dest.TribesCount, opt => opt.MapFrom(src => src.GroupMembers.Count));


        }
    }
}
