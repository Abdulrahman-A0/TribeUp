using AutoMapper;
using Domain.Entities.Users;
using Shared.DTOs.NotificationModule;

namespace Service.MappingProfiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationResponseDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<CreateNotificationDTO, Notification>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.RecipientUserId));
        }
    }
}
