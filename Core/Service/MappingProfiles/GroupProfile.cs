using AutoMapper;
using Domain.Entities.Groups;
using Service.MappingProfiles.MediaResolvers;
using Shared.DTOs.GroupMemberModule;
using Shared.DTOs.GroupMessageModule;
using Shared.DTOs.GroupModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            
            CreateMap<CreateGroupDTO, Group>();

            CreateMap<Group, GroupResultDTO>()
                .ForMember(dest => dest.MembersCount,
                    opt => opt.MapFrom(src =>
                    src.GroupMembers != null ? src.GroupMembers.Count : 0))

                .ForMember(dest => dest.GroupProfilePicture, 
                opt => opt.MapFrom<GroupProfilePictureResolver>());




            CreateMap<Group, GroupDetailsResultDTO>()
                .ForMember(dest => dest.MembersCount,
                    opt => opt.MapFrom(src =>
                    src.GroupMembers != null ? src.GroupMembers.Count : 0))

                .ForMember(dest => dest.Members,
                    opt => opt.MapFrom(src => src.GroupMembers))

                .ForMember(dest => dest.GroupProfilePicture,
                opt => opt.MapFrom<GroupProfilePictureResolver>()); 


            
            CreateMap<UpdateGroupDTO, Group>()
                .ForMember(dest => dest.GroupName,
                    opt => opt.Condition(src => src.GroupName is not null))

                .ForMember(dest => dest.Description,
                    opt => opt.Condition(src => src.Description is not null))

                .ForMember(dest => dest.Accessibility,
                    opt => opt.Condition(src => src.Accessibility is not null))

                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null));



            CreateMap<Group, GroupChatInboxDTO>()
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
                .ForMember(dest => dest.GroupProfilePicture,opt => opt.MapFrom(src => src.GroupProfilePicture))
                .ForMember(dest => dest.LastMessageContent, opt => opt.MapFrom(src => src.LastMessage.Content))
                .ForMember(dest => dest.LastMessageSenderName, opt => opt.MapFrom(src => src.LastMessage.User.UserName))
                .ForMember(dest => dest.LastMessageSentAt,  opt => opt.MapFrom(src => src.LastMessage.SentAt));

        }
    }
}
