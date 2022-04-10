using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parlivote.Core.Services.Identity;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IdentityController : Controller
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration userRegistration)
        {
            AuthenticationResult authResponse = 
                await this.identityService.RegisterAsync(userRegistration.Email, userRegistration.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.ErrorMessages
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            AuthenticationResult authResponse =
                await this.identityService.LoginAsync(userLogin.Email, userLogin.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.ErrorMessages
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token
            });
        }
    }
}
