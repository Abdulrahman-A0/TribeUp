using AutoMapper;
using Domain.Entities.Posts;
using ServiceAbstraction.Contracts;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;

namespace Service.MappingProfiles.UrlResolvers
{
    internal class PostShareUrlResolver(IPostUrlService postUrlService): IValueResolver<Post, PostFeedDTO, string>
    {
        public string Resolve(Post source, PostFeedDTO destination, string destMember, ResolutionContext context)
            => postUrlService.BuildPostUrl(source.Id);
    }
}
