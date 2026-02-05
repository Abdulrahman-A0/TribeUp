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
                // Group
                .ForMember(d => d.GroupId,
                    o => o.MapFrom(s => s.GroupId))
                .ForMember(d => d.GroupName,
                    o => o.MapFrom(s => s.Group.GroupName))
                .ForMember(d => d.GroupProfilePicture,
                    o => o.MapFrom(s => s.Group.GroupProfilePicture))

                // Sender
                .ForMember(d => d.SenderUserId,
                    o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.SenderName,
                    o => o.MapFrom(s => s.User.UserName))
                .ForMember(d => d.SenderProfilePicture,
                    o => o.MapFrom(s => s.User.ProfilePicture))

                // Message
                .ForMember(d => d.Content,
                    o => o.MapFrom(s => s.Content))
                .ForMember(d => d.SentAt,
                    o => o.MapFrom(s => s.SentAt));
        }
    }
}
