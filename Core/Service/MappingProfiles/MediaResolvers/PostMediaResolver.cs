using AutoMapper;
using Domain.Entities.Media;
using ServiceAbstraction.Contracts;
using Shared.DTOs.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;

namespace Service.MappingProfiles.MediaResolvers
{
    internal class PostMediaResolver(IMediaUrlService mediaUrlService) : IValueResolver<MediaItem, MediaItemFeedDTO, string>
    {
        public string Resolve(MediaItem source, MediaItemFeedDTO destination, string destMember, ResolutionContext context)
            => mediaUrlService.BuildUrl(source.MediaURL, MediaType.PostMedia);
    }
}
