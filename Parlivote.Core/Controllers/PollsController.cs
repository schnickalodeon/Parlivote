using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parlivote.Core.Services.Foundations.Polls;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;
using RESTFulSense.Controllers;

namespace Parlivote.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PollsController : RESTFulController
{
    private readonly IPollService pollService;
    public PollsController(IPollService pollService)
    {
        this.pollService = pollService;
    }

    [HttpPost]
    public async Task<ActionResult<Poll>> PostPollAsync([FromBody] Poll poll)
    {
        try
        {
            Poll createdPoll =
                await this.pollService.AddAsync(poll);

            return Created(createdPoll);
        }
        catch (PollValidationException pollValidationException)
        {
            return BadRequest(pollValidationException.InnerException);
        }
        catch (PollDependencyValidationException pollDependencyException)
            when (pollDependencyException.InnerException is AlreadyExistsPollException)
        {
            return Conflict(pollDependencyException.InnerException);
        }
        catch (PollDependencyException pollDependencyException)
        {
            return InternalServerError(pollDependencyException);
        }
        catch (PollServiceException pollServiceException)
        {
            return InternalServerError(pollServiceException);
        }
    }
}