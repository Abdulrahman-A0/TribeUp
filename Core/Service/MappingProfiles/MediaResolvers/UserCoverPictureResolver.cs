using AutoMapper;
using Domain.Entities.Users;
using ServiceAbstraction.Contracts;
using Shared.Enums;

namespace Service.MappingProfiles.MediaResolvers
{
    internal class UserCoverPictureResolver<TDestination>(IMediaUrlService mediaUrlService)
        : IValueResolver<ApplicationUser, TDestination, string>
    {
        public string Resolve(ApplicationUser source, TDestination destination, string destMember, ResolutionContext context)
            => mediaUrlService.BuildUrl(source.CoverPicture, MediaType.UserCover);
    }
}
