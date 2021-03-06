using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parlivote.Core.Services.Foundations.Motions;
using Parlivote.Core.Services.Processing;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using RESTFulSense.Controllers;

namespace Parlivote.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MotionsController : RESTFulController
{
    private readonly IMotionProcessingService motionProcessingService;
    public MotionsController(IMotionProcessingService motionProcessingService)
    {
        this.motionProcessingService = motionProcessingService;
    }

    [HttpPost]
    public async Task<ActionResult<Motion>> PostMotionAsync([FromBody] Motion poll)
    {
        try
        {
            Motion createdMotion =
                await this.motionProcessingService.AddAsync(poll);

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
            IQueryable<Motion> motions = this.motionProcessingService
                .RetrieveAll()
                .Include(motion => motion.Votes);

            return Ok(motions);
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

    [HttpGet("applicant/{applicantId}")]
    public ActionResult<IQueryable<Motion>> GetByApplicantIdMotions(Guid applicantId)
    {
        try
        {
            IQueryable<Motion> motions = this.motionProcessingService
                .RetrieveAll()
                .Include(motion => motion.Votes)
                .Where(motion => motion.ApplicantId == applicantId);

            return Ok(motions);
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

    [HttpGet("{motionId}")]
    public async Task<ActionResult<Motion>> GetMotionByIdAsync(Guid motionId)
    {
        try
        {
            Motion activeMotion =
                await this.motionProcessingService.RetrieveByIdAsync(motionId);

            return Ok(activeMotion);
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

    [HttpGet("Active")]
    public async Task<ActionResult<Motion>> GetActiveMotionAsync()
    {
        try
        {
            Motion activeMotion =
                await this.motionProcessingService.RetrieveActiveAsync();

            return Ok(activeMotion);
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

    [HttpPut]
    public async Task<ActionResult<Motion>> PutMotionAsync([FromBody] Motion motion)
    {
        try
        {
            Motion modifiedMotion =
                await this.motionProcessingService.ModifyAsync(motion);

            return Ok(modifiedMotion);
        }
        catch (MotionValidationException motionValidationException)
            when (motionValidationException.InnerException is NotFoundMotionException)
        {
            return NotFound(motionValidationException.InnerException);
        }
        catch (MotionValidationException motionValidationException)
        {
            return BadRequest(motionValidationException.InnerException);
        }
        catch (MotionDependencyValidationException motionDependencyValidationException)
            when (motionDependencyValidationException.InnerException is AlreadyExistsMotionException)
        {
            return Conflict(motionDependencyValidationException.InnerException);
        }
        catch (MotionDependencyException motionDependencyException)
        {
            return InternalServerError(motionDependencyException);
        }
        catch (MotionServiceException motionServiceException)
        {
            return InternalServerError(motionServiceException);
        }
    }

    [HttpDelete("{motionId}")]
    public async Task<ActionResult<Motion>> DeleteMotionById(Guid motionId)
    {
        try
        {
            Motion deletedMotion =
                await this.motionProcessingService.DeleteMotionById(motionId);

            return Ok(deletedMotion);
        }
        catch (Exception e)
        {
            //TODO Handle Excpetions
            return InternalServerError(e);
        }
    }

}