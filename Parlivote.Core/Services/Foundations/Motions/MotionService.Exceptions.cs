using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using Xeptions;

namespace Parlivote.Core.Services.Foundations.Motions;

public partial class MotionService
{
    private delegate Task<Motion> ReturningMotionFunction();
    private delegate IQueryable<Motion> ReturningMotionsFunction();

    private async Task<Motion> TryCatch(ReturningMotionFunction returningMotionFunction)
    {
        try
        {
            return await returningMotionFunction();
        }
        catch (NullMotionException nullMotionException)
        {
            throw CreateAndLogValidationException(nullMotionException);
        }
        catch (InvalidMotionException invalidMotionException)
        {
            throw CreateAndLogValidationException(invalidMotionException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            var failedMotionStorageException =
                new FailedMotionStorageException(dbUpdateException);

            throw CreateAndLogDependencyException(failedMotionStorageException);
        }
        catch (DuplicateKeyException duplicateKeyException)
        {
            var alreadyExistsMotionException =
                new AlreadyExistsMotionException(duplicateKeyException);

            throw CreateAndLogDependencyValidationException(alreadyExistsMotionException);
        }
        catch (SqlException sqlException)
        {
            var failedMotionStorageException =
                new FailedMotionStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedMotionStorageException);
        }
        catch (Exception exception)
        {
            var failedMotionServiceException =
                new FailedMotionServiceException(exception);

            throw CreateAndLogServiceException(failedMotionServiceException);
        }
    }
    private IQueryable<Motion> TryCatch(ReturningMotionsFunction returningMotionsFunction)
    {
        try
        {
            return returningMotionsFunction();
        }
        catch (SqlException sqlException)
        {
            var failedMotionStorageException =
                new FailedMotionStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedMotionStorageException);
        }
        catch (Exception exception)
        {
            var failedMotionServiceException =
                new FailedMotionServiceException(exception);

            throw CreateAndLogServiceException(failedMotionServiceException);
        }
    }

    private MotionValidationException CreateAndLogValidationException(Xeption exception)
    {
        var pollValidationException =
            new MotionValidationException(exception);

        this.loggingBroker.LogError(pollValidationException);

        return pollValidationException;
    }
    private MotionDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var pollDependencyValidationException =
            new MotionDependencyValidationException(exception);

        this.loggingBroker.LogError(pollDependencyValidationException);

        return pollDependencyValidationException;
    }
    private MotionDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var pollDependencyException =
            new MotionDependencyException(exception);

        this.loggingBroker.LogError(pollDependencyException);

        return pollDependencyException;
    }
    private MotionDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var pollDependencyException =
            new MotionDependencyException(exception);

        this.loggingBroker.LogCritical(pollDependencyException);

        return pollDependencyException;
    }
    private MotionServiceException CreateAndLogServiceException(Xeption exception)
    {
        var pollServiceException =
            new MotionServiceException(exception);

        this.loggingBroker.LogError(pollServiceException);

        return pollServiceException;
    }
}