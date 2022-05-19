using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Parlivote.Core.Services.Foundations.Users;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Identity.Users.Exceptions;
using RESTFulSense.Controllers;

namespace Parlivote.Core.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : RESTFulController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public ActionResult<IQueryable<User>> GetAllUsers()
        {
            try
            {
                IQueryable<User> users = this.userService.RetrieveAll();
                return Ok(users);
            }
            catch (UserDependencyException pollDependencyException)
            {
                return InternalServerError(pollDependencyException);
            }
            catch (UserServiceException pollServiceException)
            {
                return InternalServerError(pollServiceException);
            }
        }
    }
}
