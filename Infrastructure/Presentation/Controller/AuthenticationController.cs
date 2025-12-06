using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    public class AuthenticationController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost("Login")]
        public async Task<ActionResult<UserResultDTO>> LoginAsync(LoginDTO loginDTO)
            => Ok(await serviceManager.AuthenticationService.LoginAsync(loginDTO));

        [HttpPost("Register")]
        public async Task<ActionResult<UserResultDTO>> RegisterAsync(RegisterDTO registerDTO)
            => Ok(await serviceManager.AuthenticationService.RegisterAsync(registerDTO));
    }
}
