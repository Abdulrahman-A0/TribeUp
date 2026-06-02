using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.EventModule;

namespace Presentation.Controller
{
    [Authorize]
    public class EventsController(IServiceManager service) : ApiController
    {
        /// <summary>
        /// Creates a new event inside a group.
        /// </summary>
        /// <remarks>
        /// The event creator is automatically marked as:
        /// - Going = 1
        ///
        /// The event is automatically created with:
        /// - Upcoming = 1
        ///
        /// Required event information:
        /// - Title
        /// - Description
        /// - EventDate
        /// - Location
        ///
        /// Example:
        ///
        /// {
        ///   "title": "Movie Night",
        ///   "description": "Let's watch a movie together.",
        ///   "eventDate": "2026-06-20T20:00:00",
        ///   "location": "City Stars Cinema"
        /// }
        /// </remarks>
        /// <param name="groupId">The group in which the event will be created.</param>
        /// <param name="dto">The event information.</param>
        /// <response code="200">Event created successfully.</response>
        /// <response code="403">User is not a member of the group.</response>
        /// <response code="404">Group not found.</response>

        [HttpPost("{groupId:int}/CreateEvent")]
        public async Task<ActionResult<EventResponseDTO>> CreateEvent(
            int groupId,
            CreateEventDTO dto)
            => Ok(await service.EventService.CreateEventAsync(groupId, UserId, dto));


        /// <summary>
        /// Retrieves all events belonging to a specific group.
        /// </summary>
        /// <remarks>
        /// Returned events may have one of the following statuses:
        /// - Upcoming = 1
        /// - Completed = 2
        /// - Cancelled = 3
        ///
        /// The response includes:
        /// - Event details
        /// - Event status
        /// - Going count
        /// - Interested count
        /// - Not Going count
        ///
        /// This endpoint is typically used to populate the group's Events tab.
        /// </remarks>
        /// <param name="groupId">The target group identifier.</param>
        /// <response code="200">Events retrieved successfully.</response>
        /// <response code="404">Group not found.</response>
        
        [HttpGet("{groupId:int}/GroupEvents")]
        public async Task<ActionResult<List<EventResponseDTO>>> GetGroupEvents(
            int groupId)
            => Ok(await service.EventService.GetGroupEventsAsync(groupId));


        /// <summary>
        /// Retrieves detailed information about a specific event.
        /// </summary>
        /// <remarks>
        /// The response includes:
        /// - Event title
        /// - Description
        /// - Event date and time
        /// - Location
        /// - Event status
        /// - Attendance statistics
        /// - Creator information
        ///
        /// Possible event statuses:
        /// - Upcoming = 1
        /// - Completed = 2
        /// - Cancelled = 3
        ///
        /// This endpoint is typically used when opening the event details page.
        /// </remarks>
        /// <param name="eventId">The target event identifier.</param>
        /// <response code="200">Event retrieved successfully.</response>
        /// <response code="404">Event not found.</response>

        [HttpGet("{eventId:int}/GetEventById")]
        public async Task<ActionResult<EventResponseDTO>> GetEventById(
            int eventId)
            => Ok(await service.EventService.GetEventByIdAsync(eventId));

        /// <summary>
        /// Updates an existing event.
        /// </summary>
        /// <remarks>
        ///
        /// Only the following users may update an event:
        /// - Event creator
        /// - Group Admin
        /// - Group Owner
        ///
        /// Events with the following statuses cannot be modified:
        /// - Completed = 2
        /// - Cancelled = 3
        ///
        /// Editable fields:
        /// - Title
        /// - Description
        /// - EventDate
        /// - Location
        ///
        /// The updated event details are returned after a successful update.
        /// </remarks>
        /// <param name="eventId">The target event identifier.</param>
        /// <param name="dto">The updated event information.</param>
        /// <response code="200">Event updated successfully.</response>
        /// <response code="403">User is not allowed to update this event.</response>
        /// <response code="404">Event not found.</response>

        [HttpPut("{eventId:int}/UpdateEvent")]
        public async Task<ActionResult<EventResponseDTO>> UpdateEvent(
            int eventId,
            UpdateEventDTO dto)
            => Ok(await service.EventService.UpdateEventAsync(eventId, UserId, dto));


        /// <summary>
        /// Deletes an existing event.
        /// </summary>
        /// <remarks>
        ///
        /// Only the following users may delete an event:
        /// - Event creator
        /// - Group Admin
        /// - Group Owner
        ///
        /// When an event is deleted:
        /// - All participant responses are removed automatically.
        /// - The event becomes inaccessible.
        /// - Related attendance records are deleted.
        ///
        /// This action cannot be undone.
        /// </remarks>
        /// <param name="eventId">The target event identifier.</param>
        /// <response code="200">Event deleted successfully.</response>
        /// <response code="403">User is not allowed to delete this event.</response>
        /// <response code="404">Event not found.</response>

        [HttpDelete("{eventId:int}/DeleteEvent")]
        public async Task<IActionResult> DeleteEvent(
            int eventId)
        {
            await service.EventService.DeleteEventAsync(eventId, UserId);

            return Ok(new{Message = "Event deleted successfully."});
        }

        /// <summary>
        /// Responds to an event invitation.
        /// </summary>
        /// <remarks>
        /// Possible status values:
        /// - Going = 1 : User plans to attend.
        /// - Interested = 2 : User is interested but not committed.
        /// - NotGoing = 3 : User does not plan to attend.
        ///
        /// If the user has already responded, the existing response will be updated.
        ///
        /// Example:
        ///
        /// POST /api/events/15/RespondToEvent
        ///
        /// {
        ///   "status": 1
        /// }
        /// </remarks>
        /// <param name="eventId">The target event identifier.</param>
        /// <param name="dto">Contains the user's attendance status.</param>
        /// <response code="200">Response updated successfully.</response>
        /// <response code="400">Event is completed or cancelled.</response>
        /// <response code="403">User is not a member of the group.</response>
        /// <response code="404">Event not found.</response>
        
        [HttpPost("{eventId:int}/RespondToEvent")]
        public async Task<IActionResult> RespondToEvent(
            int eventId,
            RespondToEventDTO dto)
        {
            await service.EventService.RespondToEventAsync(eventId, UserId, dto.Status);

            return Ok(new{Message = "Response updated successfully."});
        }
    }
}