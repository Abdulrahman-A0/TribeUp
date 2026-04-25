using AutoMapper;
using Domain.Entities.Stories;
using ServiceAbstraction.Contracts;
using Shared.DTOs.StoriesModule;
using Shared.Enums;

namespace Service.MappingProfiles.MediaResolvers
{
    internal class StoryMediaResolver(IMediaUrlService mediaUrlService)
        : IValueResolver<Story, StoryResponseDTO, string>
    {
        public string Resolve(Story source, StoryResponseDTO destination, string destMember, ResolutionContext context)
            => mediaUrlService.BuildUrl(source.MediaURL, MediaType.StoryMedia);
    }
}