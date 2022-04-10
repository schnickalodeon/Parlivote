using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parlivote.Core.Services.Identity;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Exceptions;

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
            try
            {
                AuthSuccessResponse authSuccessResponse =
                    await this.identityService.RegisterAsync(userRegistration.Email, userRegistration.Password);

                return Ok(authSuccessResponse);
            }
            catch (UserAlreadyExistsException userAlreadyExistsException)
            {
                return BadRequest(userAlreadyExistsException.Message);
            }
            catch (InvalidEmailException invalidEmailException)
            {
                return BadRequest(invalidEmailException.Message);
            }
            catch (UserRegistrationException userRegistrationException)
            {
                return BadRequest(userRegistrationException.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            try
            {
                AuthSuccessResponse authResponse =
                    await this.identityService.LoginAsync(userLogin.Email, userLogin.Password);

                return Ok(authResponse);
            }
            catch (InvalidEmailException invalidEmailException)
            {
                return BadRequest(invalidEmailException.Message);
            }
            catch (InvalidEmailPasswordCombinationException invalidEmailPasswordCombinationException)
            {
                return BadRequest(invalidEmailPasswordCombinationException.Message);
            }
          
        }
    }
}
