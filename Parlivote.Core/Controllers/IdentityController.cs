using System;
using System.Linq;
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
        public async Task<ActionResult<AuthenticationResult>> Register([FromBody] UserRegistration userRegistration)
        {
            try
            {
                AuthSuccessResponse authSuccessResponse =
                    await this.identityService.RegisterAsync(userRegistration.Email, userRegistration.Password);

                return Ok(authSuccessResponse);
            }
            catch (UserAlreadyExistsException userAlreadyExistsException)
            {
                var authFailedResponse = new AuthFailedResponse(userAlreadyExistsException.Message);
                return BadRequest(authFailedResponse);
            }
            catch (InvalidEmailException invalidEmailException)
            {
                var authFailedResponse = new AuthFailedResponse(invalidEmailException.Message);
                return BadRequest(authFailedResponse);
            }
            catch (UserRegistrationException userRegistrationException)
            {
                var authFailedResponse = new AuthFailedResponse(userRegistrationException.Errors.ToList());
                return BadRequest(authFailedResponse);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResult>> Login([FromBody] UserLogin userLogin)
        {
            try
            {
                AuthSuccessResponse authResponse =
                    await this.identityService.LoginAsync(userLogin.Email, userLogin.Password);

                return Ok(authResponse);
            }
            catch (InvalidEmailException invalidEmailException)
            {
                var authFailedResponse = new AuthFailedResponse(invalidEmailException.Message);
                return BadRequest(authFailedResponse);
            }
            catch (InvalidEmailPasswordCombinationException invalidEmailPasswordCombinationException)
            {
                var authFailedResponse = new AuthFailedResponse(invalidEmailPasswordCombinationException.Message);
                return BadRequest(authFailedResponse);
            }
          
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticationResult>> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                AuthSuccessResponse authResponse =
                    await this.identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

                return Ok(authResponse);
            }
            catch (InvalidEmailException invalidEmailException)
            {
                var authFailedResponse = new AuthFailedResponse(invalidEmailException.Message);
                return BadRequest(authFailedResponse);
            }
            catch (InvalidEmailPasswordCombinationException invalidEmailPasswordCombinationException)
            {
                var authFailedResponse = new AuthFailedResponse(invalidEmailPasswordCombinationException.Message);
                return BadRequest(authFailedResponse);
            }

        }
    }
}
