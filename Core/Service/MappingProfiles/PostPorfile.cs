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
            CreateMap<Post, PostFeedDTO>()
                 .ForMember(dest => dest.PostId,
                     opt => opt.MapFrom(src => src.Id))

                 .ForMember(dest => dest.GroupId,
                     opt => opt.MapFrom(src => src.GroupId))

                 .ForMember(dest => dest.GroupName,
                     opt => opt.MapFrom(src => src.Group.GroupName))

                 .ForMember(dest => dest.LikesCount,
                     opt => opt.MapFrom(src => src.Likes.Count))

                 .ForMember(dest => dest.CommentCount,
                     opt => opt.MapFrom(src => src.Comments.Count))

                 .ForMember(dest => dest.CreatedAt,
                     opt => opt.MapFrom(src => src.CreatedAt))

                 .ForMember(dest => dest.Media,
                     opt => opt.MapFrom(src => src.MediaItems))

                 .ForMember(dest => dest.IsLikedByCurrentUser,
                     opt => opt.Ignore())

                 .ForMember(dest => dest.GroupRelation,
                     opt => opt.Ignore())

                 .ForMember(dest => dest.FeedScore,
                     opt => opt.Ignore());

            CreateMap<CreatePostDTO, Post>();

            CreateMap<MediaItem, MediaItemDTO>();
        }
    }
}
