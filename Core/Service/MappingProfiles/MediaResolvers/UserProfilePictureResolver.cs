using AutoMapper;
using Domain.Entities.Users;
using ServiceAbstraction.Contracts;
using Shared.DTOs.ProfileModule;
using Shared.Enums;

namespace Service.MappingProfiles.MediaResolvers
{
    internal class UserProfilePictureResolver(IMediaUrlService mediaUrlService) : IValueResolver<ApplicationUser, UserProfileDTO, string>
    {
        public string Resolve(ApplicationUser source, UserProfileDTO destination, string destMember, ResolutionContext context)
            => mediaUrlService.BuildUrl(source.ProfilePicture, MediaType.UserProfile);
    }
}
