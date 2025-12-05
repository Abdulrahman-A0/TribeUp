using AutoMapper;
using Domain.Entities.Users;
using Shared.DTOs.IdentityModule;

namespace Service.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserResultDTO>()
                .ForMember(dest => dest.FullName, options => options.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<RegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, options => options.Ignore());
        }
    }
}
