using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.NotificationModule;

namespace Presentation.Controller
{
    [Authorize]
    public class NotificationController(IServiceManager serviceManager) : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<PagedNotificationsDTO>> GetMyNotificationAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await serviceManager.NotificationService.GetMyNotificationsAsync(UserId, pageNumber, pageSize));

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await serviceManager
                .NotificationService
                .MarkAsReadAsync(id, UserId);

            return NoContent();
        }
    }
}
