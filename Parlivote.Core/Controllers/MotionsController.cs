using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parlivote.Core.Services.Foundations.Motions;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using RESTFulSense.Controllers;

namespace Parlivote.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MotionsController : RESTFulController
{
    private readonly IMotionService pollService;
    public MotionsController(IMotionService pollService)
    {
        this.pollService = pollService;
    }

    [HttpPost]
    public async Task<ActionResult<Motion>> PostMotionAsync([FromBody] Motion poll)
    {
        try
        {
            Motion createdMotion =
                await this.pollService.AddAsync(poll);

            return Created(createdMotion);
        }
        catch (MotionValidationException pollValidationException)
        {
            return BadRequest(pollValidationException.InnerException);
        }
        catch (MotionDependencyValidationException pollDependencyException)
            when (pollDependencyException.InnerException is AlreadyExistsMotionException)
        {
            return Conflict(pollDependencyException.InnerException);
        }
        catch (MotionDependencyException pollDependencyException)
        {
            return InternalServerError(pollDependencyException);
        }
        catch (MotionServiceException pollServiceException)
        {
            return InternalServerError(pollServiceException);
        }
    }

    [HttpGet]
    public ActionResult<IQueryable<Motion>> GetAllMotions()
    {
        try
        {
            IQueryable<Motion> polls = this.pollService.RetrieveAll();
            return Ok(polls);
        }
        catch (MotionDependencyException pollDependencyException)
        {
            return InternalServerError(pollDependencyException);
        }
        catch (MotionServiceException pollServiceException)
        {
            return InternalServerError(pollServiceException);
        }
    }
}