using AutoMapper;
using Domain.Entities.Groups;
using Service.MappingProfiles.MediaResolvers;
using Shared.DTOs.GroupInvitationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class GroupInvitationProfile : Profile
    {
        public GroupInvitationProfile()
        {

            CreateMap<GroupInvitation, InvitationResultDTO>()
                .ForMember(dest => dest.InvitationUrl,
                opt => opt.MapFrom((src, dest, destMember, context) =>
                $"{context.Items["FrontUrl"]}/tribes/join/{src.Token}"));



            CreateMap<CreateInvitationDTO, GroupInvitation>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.GroupId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());


            CreateMap<Group, InvitationDetailsDTO>()
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GroupPicture, opt => opt.MapFrom<GroupProfilePictureResolver>());
        }
    }
}
