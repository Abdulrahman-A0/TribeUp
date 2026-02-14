using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.IdentityModule;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
            await serviceManager.AuthenticationService
                .LogoutAsync(UserId!, deviceId);

            return NoContent();
        }

        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            await serviceManager.AuthenticationService
                .ChangePasswordAsync(UserId, changePasswordDTO);

            return NoContent();
        }

        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            await serviceManager.AuthenticationService.ForgotPasswordAsync(forgotPasswordDTO);
            return NoContent();
        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            await serviceManager.AuthenticationService.ResetPasswordAsync(resetPasswordDTO);
            return NoContent();
        }

    }
}
