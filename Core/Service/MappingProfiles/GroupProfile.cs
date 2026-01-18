using AutoMapper;
using Domain.Entities.Groups;
using Shared.DTOs.GroupMemberModule;
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
            // =========================
            // Group mappings
            // =========================
            CreateMap<CreateGroupDTO, Group>();

            CreateMap<Group, GroupResultDTO>()
                .ForMember(dest => dest.MembersCount,
                    opt => opt.MapFrom(src =>
                    src.GroupMembers != null ? src.GroupMembers.Count : 0));



            CreateMap<Group, GroupDetailsResultDTO>()
                .ForMember(dest => dest.MembersCount,
                    opt => opt.MapFrom(src =>
                    src.GroupMembers != null ? src.GroupMembers.Count : 0))
                .ForMember(dest => dest.Members,
                    opt => opt.MapFrom(src => src.GroupMembers));


            
            CreateMap<UpdateGroupDTO, Group>()
                .ForMember(dest => dest.GroupName,
                    opt => opt.Condition(src => src.GroupName is not null))

                .ForMember(dest => dest.Description,
                    opt => opt.Condition(src => src.Description is not null))

                .ForMember(dest => dest.GroupProfilePicture,
                    opt => opt.Condition(src => src.GroupProfilePicture is not null))

                .ForMember(dest => dest.Accessibility,
                    opt => opt.Condition(src => src.Accessibility is not null))

                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null));
        }
    }
}
