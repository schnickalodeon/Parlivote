using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parlivote.Core.Services.Foundations.Users;
using Parlivote.Shared.Models.Identity.Exceptions;
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

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUserById(Guid userId)
        {
            try
            {
                User user = await this.userService.RetrieveByIdAsync(userId);
                return Ok(user);
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

        [HttpPut]
        public async Task<ActionResult<User>> PutUserByAsync(User user)
        {
            try
            {
                User updatedUser =
                    await this.userService.ModifyUserAsync(user);

                return Ok(updatedUser);
            }
            catch (NullUserException nullUserException)
            {
                return BadRequest(nullUserException);
            }
            catch (UserNotFoundException userNotFoundException)
            {
                return BadRequest(userNotFoundException);
            }
        }
    }
}
