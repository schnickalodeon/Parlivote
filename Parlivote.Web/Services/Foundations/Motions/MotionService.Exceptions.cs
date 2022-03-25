using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;

namespace Parlivote.Web.Services.Foundations.Motions;

public partial class MotionService
{
    private delegate Task<Motion> ReturningMotionFunction();
    private delegate Task<List<Motion>> ReturningMotionsFunction();

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
            var failedMotionServiceException =
                new FailedMotionServiceException(exception);

            throw CreateAndLogServiceException(failedMotionServiceException);
        }
    }
    private async Task<List<Motion>> TryCatch(ReturningMotionsFunction returningMotionsFunction)
    {
        try
        {
            return await returningMotionsFunction();
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