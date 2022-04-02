using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using Xeptions;

namespace Parlivote.Core.Services.Foundations.Meetings;

public partial class MeetingService
{
    private delegate Task<Meeting> ReturningMeetingFunction();
    private delegate IQueryable<Meeting> ReturningMeetingsFunction();

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
        catch (InvalidMeetingException invalidMeetingException)
        {
            throw CreateAndLogValidationException(invalidMeetingException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            var lockedMeetingException =
                new LockedMeetingException(dbUpdateConcurrencyException);

            throw CreateAndLogDependencyValidationException(lockedMeetingException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            var failedMeetingStorageException =
                new FailedMeetingStorageException(dbUpdateException);

            throw CreateAndLogDependencyException(failedMeetingStorageException);
        }
        catch (DuplicateKeyException duplicateKeyException)
        {
            var alreadyExistsMeetingException =
                new AlreadyExistsMeetingException(duplicateKeyException);

            throw CreateAndLogDependencyValidationException(alreadyExistsMeetingException);
        }
        catch (SqlException sqlException)
        {
            var failedMeetingStorageException =
                new FailedMeetingStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedMeetingStorageException);
        }
        catch (Exception exception)
        {
            var failedMeetingServiceException =
                new FailedMeetingServiceException(exception);

            throw CreateAndLogServiceException(failedMeetingServiceException);
        }
    }
    private IQueryable<Meeting> TryCatch(ReturningMeetingsFunction returningMeetingsFunction)
    {
        try
        {
            return returningMeetingsFunction();
        }
        catch (SqlException sqlException)
        {
            var failedMotionStorageException =
                new FailedMeetingStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedMotionStorageException);
        }
        catch (Exception exception)
        {
            var failedMotionServiceException =
                new FailedMeetingServiceException(exception);

            throw CreateAndLogServiceException(failedMotionServiceException);
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