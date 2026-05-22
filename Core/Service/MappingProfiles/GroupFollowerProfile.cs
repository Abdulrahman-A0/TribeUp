using AutoMapper;
using Domain.Entities.Groups;
using Shared.DTOs.GroupFollowerModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class GroupFollowerProfile : Profile
    {
        public GroupFollowerProfile()
        {
            CreateMap<GroupFollowers, GroupFollowerResultDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.FollowedAt, opt => opt.MapFrom(src => src.FollowedAt))

                
                .ForMember(dest => dest.ProfilePictureUrl,
                    opt => opt.MapFrom(
                        (src, dest, destMember, context) =>
                        context.Mapper.Map<string>(src.User)));
        }
    }
}
