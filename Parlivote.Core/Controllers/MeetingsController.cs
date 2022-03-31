using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
}