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
    //[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    //[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
    //[ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public class ApiController : ControllerBase
    {
        protected string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
