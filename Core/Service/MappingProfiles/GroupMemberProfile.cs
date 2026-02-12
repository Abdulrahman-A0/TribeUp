using AutoMapper;
using Domain.Entities.Groups;
using Shared.DTOs.GroupMemberModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class GroupMemberProfile : Profile
    {
        public GroupMemberProfile()
        {

            CreateMap<GroupMembers, GroupMemberResultDTO>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src =>
                    src.User != null ? src.User.UserName : string.Empty))
                .ForMember(dest => dest.UserProfilePicture,
                    opt => opt.MapFrom(src =>
                    src.User != null ? src.User.ProfilePicture : string.Empty));
        }
    }
}
