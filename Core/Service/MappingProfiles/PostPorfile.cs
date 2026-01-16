using AutoMapper;
using Domain.Entities.Media;
using Domain.Entities.Posts;
using Shared.DTOs.Posts;

namespace Service.MappingProfiles
{
    public class PostPorfile : Profile
    {
        public PostPorfile()
        {
            CreateMap<Post, FeedPostDTO>()
                .ForMember(dest => dest.PostId, options => options.MapFrom(src => src.Id))
                .ForMember(dest => dest.GroupName, options => options.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.LikesCount, options => options.MapFrom(src => src.Likes.Count))
                .ForMember(dest => dest.CommentCount, options => options.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.CreatedAt, options => options.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Media, options => options.MapFrom(src => src.MediaItems));

            CreateMap<CreatePostDTO, Post>();

            CreateMap<MediaItem, MediaItemDTO>();

        }
    }
}
