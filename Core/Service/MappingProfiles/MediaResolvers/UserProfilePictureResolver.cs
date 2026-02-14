using AutoMapper;
using Domain.Entities.Users;
using ServiceAbstraction.Contracts;
using Shared.DTOs.ProfileModule;
using Shared.Enums;

namespace Service.MappingProfiles.MediaResolvers
{
    internal class UserProfilePictureResolver(IMediaUrlService mediaUrlService)
        : IMemberValueResolver<object, object, string?, string>
    {
        public string Resolve(object source, object destination, string? sourceMember, string destMember, ResolutionContext context)
            => mediaUrlService.BuildUrl(sourceMember, MediaType.UserProfile)!;
    }
}
