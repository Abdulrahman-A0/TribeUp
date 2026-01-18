using AutoMapper;
using Domain.Entities.Groups;
using Shared.DTOs.GroupJoinRequestModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class GroupJoinRequestProfile : Profile
    {
        public GroupJoinRequestProfile()
        {
            // Map GroupJoinRequest entity to GroupJoinRequestResultDTO
            CreateMap<GroupJoinRequest, GroupJoinRequestResultDTO>()
                .ForMember(dest => dest.GroupName,
                    opt => opt.MapFrom(src =>
                    src.Group != null ? src.Group.GroupName : string.Empty))

                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src =>
                    src.User != null ? src.User.UserName : string.Empty));
        }
    }
}
