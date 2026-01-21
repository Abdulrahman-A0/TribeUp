using AutoMapper;
using Domain.Entities.Groups;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupModule;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles.MediaResolvers
{
    internal class GroupProfilePictureResolver(IMediaUrlService mediaUrlService) : IValueResolver<Group, object, string>
    {
        public string Resolve(Group source, object destination, string destMember, ResolutionContext context)
            => mediaUrlService.BuildUrl(source.GroupProfilePicture,MediaType.GroupProfile);
    }
}
