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
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom<UserProfilePictureResolver>());

            CreateMap<RegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<ApplicationUser, string>().ConvertUsing((src, dest, context) =>
                context.Mapper.Map<UserProfileDTO>(src).ProfilePicture);
        }
    }
}
