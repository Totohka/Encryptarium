using Encryptarium.BL.Service.Secrets;
using Encryptarium.Domain.DTO.Auth;
using Encryptarium.Domain.Model.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Encryptarium.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) 
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (await _authService.IsValidUserAsync(model))
            {
                return Ok(await _authService.GenerateTokenAsync(model.UserName));
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var userForReg = new eUser()
            {
                UserName = model.UserName,
            };
            var result = await _authService.CreateUserAsync(userForReg, model.Password);
            if (result.Succeeded)
            {
                return Created();
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }
                return ValidationProblem(statusCode: 400, modelStateDictionary: ModelState);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenDto token)
        {
            try
            {
                var refreshedToken = await _authService.RefreshTokenAsync(token);
                return Ok(refreshedToken);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Token", ex.Message);
                return ValidationProblem(statusCode: 400, modelStateDictionary: ModelState);
            }
        }
    }
}
