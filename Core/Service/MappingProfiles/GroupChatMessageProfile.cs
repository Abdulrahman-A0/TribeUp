using AutoMapper;
using Domain.Entities.Groups;
using Shared.DTOs.GroupMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class GroupChatMessageProfile : Profile
    {
        public GroupChatMessageProfile()
        {
            CreateMap<GroupChatMessage, GroupMessageResponseDTO>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User.UserName));
        }
    }
}
