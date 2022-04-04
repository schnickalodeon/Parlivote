using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;

namespace Parlivote.Web.Services.Foundations.Meetings;

public partial class MeetingService
{
    private delegate Task<Meeting> ReturningMeetingFunction();
    private delegate Task<List<Meeting>> ReturningMeetingsFunction();

    private async Task<Meeting> TryCatch(ReturningMeetingFunction returningMeetingFunction)
    {
        try
        {
            return await returningMeetingFunction();
        }
        catch (NullMeetingException nullMeetingException)
        {
            throw CreateAndLogValidationException(nullMeetingException);
        }
        catch (NotFoundMeetingException notFoundMeetingException)
        {
            throw CreateAndLogValidationException(notFoundMeetingException);
        }
        catch (InvalidMeetingException invalidMeetingException)
        {
            throw CreateAndLogValidationException(invalidMeetingException);
        }
        catch (HttpResponseUrlNotFoundException httpResponseUrlNotFoundException)
        {
            throw CreateAndLogCriticalDependencyException(httpResponseUrlNotFoundException);
        }
        catch (HttpResponseUnauthorizedException httpUnauthorizedException)
        {
            throw CreateAndLogCriticalDependencyException(httpUnauthorizedException);
        }
        catch (HttpResponseConflictException httpConflictException)
        {
            throw CreateAndLogDependencyValidationException(httpConflictException);
        }
        catch (HttpResponseBadRequestException httpBadRequestException)
        {
            throw CreateAndLogDependencyValidationException(httpBadRequestException);
        }
        catch (HttpResponseInternalServerErrorException httpInternalServerErrorException)
        {
            throw CreateAndLogDependencyException(httpInternalServerErrorException);
        }
        catch (HttpResponseException httpResponseException)
        {
            throw CreateAndLogDependencyException(httpResponseException);
        }
        catch (Exception exception)
        {
            var failedMeetingServiceException =
                new FailedMeetingServiceException(exception);

            throw CreateAndLogServiceException(failedMeetingServiceException);
        }
    }
    private async Task<List<Meeting>> TryCatch(ReturningMeetingsFunction returningMeetingsFunction)
    {
        try
        {
            return await returningMeetingsFunction();
        }
        catch (HttpResponseUrlNotFoundException httpResponseUrlNotFoundException)
        {
            throw CreateAndLogCriticalDependencyException(httpResponseUrlNotFoundException);
        }
        catch (HttpResponseUnauthorizedException httpUnauthorizedException)
        {
            throw CreateAndLogCriticalDependencyException(httpUnauthorizedException);
        }
        catch (HttpResponseInternalServerErrorException httpInternalServerErrorException)
        {
            throw CreateAndLogDependencyException(httpInternalServerErrorException);
        }
        catch (HttpResponseException httpResponseException)
        {
            throw CreateAndLogDependencyException(httpResponseException);
        }
        catch (Exception exception)
        {
            var failedMeetingServiceException =
                new FailedMeetingServiceException(exception);

            throw CreateAndLogServiceException(failedMeetingServiceException);
        }
    }
    private MeetingValidationException CreateAndLogValidationException(Xeption exception)
    {
        var meetingValidationException =
            new MeetingValidationException(exception);

        this.loggingBroker.LogError(meetingValidationException);

        return meetingValidationException;
    }
    private MeetingDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var meetingDependencyValidationException =
            new MeetingDependencyValidationException(exception);

        this.loggingBroker.LogError(meetingDependencyValidationException);

        return meetingDependencyValidationException;
    }
    private MeetingDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var meetingDependencyException =
            new MeetingDependencyException(exception);

        this.loggingBroker.LogError(meetingDependencyException);

        return meetingDependencyException;
    }
    private MeetingDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var meetingDependencyException =
            new MeetingDependencyException(exception);

        this.loggingBroker.LogCritical(meetingDependencyException);

        return meetingDependencyException;
    }
    private MeetingServiceException CreateAndLogServiceException(Xeption exception)
    {
        var meetingServiceException =
            new MeetingServiceException(exception);

        this.loggingBroker.LogError(meetingServiceException);

        return meetingServiceException;
    }
}