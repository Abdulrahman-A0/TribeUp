using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    public class AuthenticationController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponseDTO>> LoginAsync(
            [FromBody] LoginDTO loginDTO,
            [FromHeader(Name = "X-Device-Id")] string deviceId)
            => Ok(await serviceManager.AuthenticationService.LoginAsync(loginDTO, deviceId));

        [HttpPost("Register")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterAsync(
            [FromBody] RegisterDTO registerDTO,
            [FromHeader(Name = "X-Device-Id")] string deviceId)
            => Ok(await serviceManager.AuthenticationService.RegisterAsync(registerDTO, deviceId));

        [HttpPost("Refresh")]
        public async Task<ActionResult<AuthResponseDTO>> RefreshAsync(RefreshTokenRequestDTO refreshTokenDTO)
            => Ok(await serviceManager.AuthenticationService.RefreshAsync(refreshTokenDTO));

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync([FromHeader(Name = "X-Device-Id")] string deviceId)
        {
            var userId = GetCurrentUserId();

            await serviceManager.AuthenticationService
                .LogoutAsync(userId!, deviceId);

            return NoContent();
        }

    }
}
