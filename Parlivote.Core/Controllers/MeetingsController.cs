using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Parlivote.Core.Services.Foundations.Meetings;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using RESTFulSense.Controllers;

namespace Parlivote.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MeetingsController : RESTFulController
{
    private readonly IMeetingService meetingService;
    public MeetingsController(IMeetingService meetingService)
    {
        this.meetingService = meetingService;
    }

    [HttpPost]
    public async Task<ActionResult<Meeting>> PostMeetingAsync([FromBody] Meeting meeting)
    {
        try
        {
            Meeting createdMeeting =
                await this.meetingService.AddAsync(meeting);

            return Created(createdMeeting);
        }
        catch (MeetingValidationException meetingValidationException)
        {
            return BadRequest(meetingValidationException.InnerException);
        }
        catch (MeetingDependencyValidationException meetingDependencyException)
            when (meetingDependencyException.InnerException is AlreadyExistsMeetingException)
        {
            return Conflict(meetingDependencyException.InnerException);
        }
        catch (MeetingDependencyException meetingDependencyException)
        {
            return InternalServerError(meetingDependencyException);
        }
        catch (MeetingServiceException meetingServiceException)
        {
            return InternalServerError(meetingServiceException);
        }
    }

    [HttpGet]
    public ActionResult<IQueryable<Meeting>> GetAllMeetings()
    {
        try
        {
            IQueryable<Meeting> meetings = this.meetingService.RetrieveAll();
            return Ok(meetings);
        }
        catch (MeetingDependencyException meetingDependencyException)
        {
            return InternalServerError(meetingDependencyException);
        }
        catch (MeetingServiceException meetingServiceException)
        {
            return InternalServerError(meetingServiceException);
        }
    }

    [HttpGet("{meetingId}")]
    public async Task<ActionResult<Meeting>> GetMeetingByIdAsync(Guid meetingId)
    {
        try
        {
            Meeting meeting =
                await this.meetingService.RetrieveByIdAsync(meetingId);

            return Ok(meeting);
        }
        catch (MeetingValidationException meetingValidationException)
            when (meetingValidationException.InnerException is NotFoundMeetingException)
        {
            return NotFound(meetingValidationException.InnerException);
        }
        catch (MeetingValidationException meetingValidationException)
        {
            return BadRequest(meetingValidationException.InnerException);
        }
        catch (MeetingDependencyException meetingDependencyException)
        {
            return InternalServerError(meetingDependencyException.Message);
        }
        catch (MeetingServiceException meetingServiceException)
        {
            return InternalServerError(meetingServiceException.Message);
        }
    }

    [HttpGet("WithMotions")]
    public async Task<ActionResult<List<Meeting>>> GetAllMeetingsWithMotions()
    {
        try
        {
            IQueryable<Meeting> meetings = this.meetingService
                .RetrieveAll()
                .Include(meeting => meeting.Motions);

            List<Meeting> meetingsWithMotions =
                await meetings.ToListAsync();

            return Ok(meetingsWithMotions);
        }
        catch (MeetingDependencyException meetingDependencyException)
        {
            return InternalServerError(meetingDependencyException);
        }
        catch (MeetingServiceException meetingServiceException)
        {
            return InternalServerError(meetingServiceException);
        }
    }

    [HttpGet("{meetingId}/WithMotions")]
    public async Task<ActionResult<Meeting>> GetMeetingByIdWithMotions(Guid meetingId)
    {
        try
        {
            Meeting meeting = await this.meetingService
                .RetrieveAll()
                .Include(meeting => meeting.Motions)
                .FirstOrDefaultAsync(meeting => meeting.Id == meetingId);

            return Ok(meeting);
        }
        catch (MeetingDependencyException meetingDependencyException)
        {
            return InternalServerError(meetingDependencyException);
        }
        catch (MeetingServiceException meetingServiceException)
        {
            return InternalServerError(meetingServiceException);
        }
    }

    [HttpPut]
    public async Task<ActionResult<Meeting>> PutMeetingAsync([FromBody] Meeting meeting)
    {
        try
        {
            Meeting modifiedMeeting =
                await this.meetingService.ModifyAsync(meeting);

            return Ok(modifiedMeeting);
        }
        catch (MeetingValidationException meetingValidationException)
            when (meetingValidationException.InnerException is NotFoundMeetingException)
        {
            return NotFound(meetingValidationException.InnerException);
        }
        catch (MeetingValidationException meetingValidationException)
        {
            return BadRequest(meetingValidationException.InnerException);
        }
        catch (MeetingDependencyValidationException meetingDependencyValidationException)
            when (meetingDependencyValidationException.InnerException is AlreadyExistsMeetingException)
        {
            return Conflict(meetingDependencyValidationException.InnerException);
        }
        catch (MeetingDependencyException meetingDependencyException)
        {
            return InternalServerError(meetingDependencyException);
        }
        catch (MeetingServiceException meetingServiceException)
        {
            return InternalServerError(meetingServiceException);
        }
    }

    [HttpDelete("{meetingId}")]
    public async Task<ActionResult<Meeting>> DeleteMeetingById(Guid meetingId)
    {
        try
        {
            Meeting deletedMeeting =
                await this.meetingService.DeleteMeetingById(meetingId);

            return Ok(deletedMeeting);
        }
        catch (Exception e)
        {
           //TODO Handle Excpetions
           return InternalServerError(e);
        }
    }
}