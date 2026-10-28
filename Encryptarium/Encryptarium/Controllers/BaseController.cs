using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Encryptarium.Controllers
{
    public class BaseController : ControllerBase
    {
        protected async Task<IActionResult> BaseRequest(Func<int, Task<IActionResult>> func)
        {
            try
            {
                int? userId = GetUserId();
                if (userId.HasValue)
                {
                    return await func(userId.Value);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        protected int? GetUserId()
        {
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(Request.Headers.Authorization[0].Substring(7));
                return int.Parse(jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
