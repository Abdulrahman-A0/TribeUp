using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]

    public class ApiController : ControllerBase
    {
        protected string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
