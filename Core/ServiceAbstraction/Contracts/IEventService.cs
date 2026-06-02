using Shared.DTOs.EventModule;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IEventService
    {
        Task<EventResponseDTO> CreateEventAsync(
            int groupId,
            string userId,
            CreateEventDTO dto);

        Task<List<EventResponseDTO>> GetGroupEventsAsync(
            int groupId);

        Task<EventResponseDTO?> GetEventByIdAsync(
            int eventId);

        Task<EventResponseDTO> UpdateEventAsync(
            int eventId,
            string userId,
            UpdateEventDTO dto);

        Task DeleteEventAsync(
            int eventId,
            string userId);

        Task RespondToEventAsync(
            int eventId,
            string userId,
            EventResponseStatus status);
    }
}
