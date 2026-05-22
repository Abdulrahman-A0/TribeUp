using AutoMapper;
using Domain.Entities.Groups;
using Shared.DTOs.GroupMemberModule;
using Shared.DTOs.ProfileModule;
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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))

                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))

                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                
                .ForMember(dest => dest.UserProfilePicture, opt => opt.MapFrom((src, dest, destMember, context) => {
                    var userDto = context.Mapper.Map<UserProfileDTO>(src.User);
                    return userDto?.ProfilePicture ?? string.Empty;
                }));
        }
    }
}
