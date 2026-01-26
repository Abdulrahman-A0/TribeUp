using AutoMapper;
using Domain.Entities.Media;
using Domain.Entities.Posts;
using Microsoft.Extensions.Configuration;
using Service.MappingProfiles.MediaResolvers;
using Service.MappingProfiles.UrlResolvers;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;

namespace Service.MappingProfiles
{
    public class PostPorfile : Profile
    {
        public PostPorfile()
        {
            //Feed

            CreateMap<Post, PostFeedDTO>()
                 .ForMember(dest => dest.PostId,
                     opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.UserId,
                     opt => opt.MapFrom(src => src.User.Id))
                 .ForMember(dest => dest.Username,
                     opt => opt.MapFrom(src => src.User.UserName))
                 .ForMember(dest => dest.GroupId,
                     opt => opt.MapFrom(src => src.GroupId))
                 .ForMember(dest => dest.GroupName,
                     opt => opt.MapFrom(src => src.Group.GroupName))
                 .ForMember(dest => dest.GroupProfilePicture,
                     opt => opt.MapFrom(src => src.Group.GroupProfilePicture))
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
                 .ForMember(dest => dest.FeedScore,
                     opt => opt.Ignore());

            // Create Post

            CreateMap<CreatePostDTO, Post>()
                  .ForMember(dest => dest.UserId,
                      opt => opt.Ignore());

            CreateMap<MediaItem, MediaItemFeedDTO>()
                .ForMember(dest => dest.MediaURL,
                    opt => opt.MapFrom<PostMediaResolver>());


            // Create Comment
            CreateMap<Comment, CommentResultDTO>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PostId,
                    opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ProfilePicture,
                    opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.Content,
                    opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt));


            // Get Likes
            CreateMap<Like, LikeResultDTO>()
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ProfilePicture,
                    opt => opt.MapFrom(src => src.User.ProfilePicture));


        }
    }
}