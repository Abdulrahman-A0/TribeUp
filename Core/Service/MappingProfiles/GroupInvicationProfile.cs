using AutoMapper;
using Domain.Entities.Groups;
using Shared.DTOs.GroupInvitationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class GroupInvicationProfile : Profile
    {
        public GroupInvicationProfile()
        {

            CreateMap<GroupInvitation, InvitationResultDTO>()
                .ForMember(dest => dest.InvitationUrl,
                opt => opt.MapFrom((src, dest, destMember, context) =>
                $"{context.Items["FrontUrl"]}/groups/join/{src.Token}"));
                
                

            CreateMap<CreateInvitationDTO, GroupInvitation>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.GroupId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
