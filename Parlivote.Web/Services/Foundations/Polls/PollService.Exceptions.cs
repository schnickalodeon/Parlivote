using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;

namespace Parlivote.Web.Services.Foundations.Polls;

public partial class PollService
{
    private delegate Task<Poll> ReturningPollFunction();
    private delegate Task<List<Poll>> ReturningPollsFunction();

    private async Task<Poll> TryCatch(ReturningPollFunction returningPollFunction)
    {
        try
        {
            return await returningPollFunction();
        }
        catch (NullPollException nullPollException)
        {
            throw CreateAndLogValidationException(nullPollException);
        }
        catch (InvalidPollException invalidPollException)
        {
            throw CreateAndLogValidationException(invalidPollException);
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
            var failedPollServiceException =
                new FailedPollServiceException(exception);

            throw CreateAndLogServiceException(failedPollServiceException);
        }
    }
    private async Task<List<Poll>> TryCatch(ReturningPollsFunction returningPollsFunction)
    {
        try
        {
            return await returningPollsFunction();
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
            var failedPollServiceException =
                new FailedPollServiceException(exception);

            throw CreateAndLogServiceException(failedPollServiceException);
        }
    }
    private PollValidationException CreateAndLogValidationException(Xeption exception)
    {
        var pollValidationException =
            new PollValidationException(exception);

        this.loggingBroker.LogError(pollValidationException);

        return pollValidationException;
    }
    private PollDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var pollDependencyValidationException =
            new PollDependencyValidationException(exception);

        this.loggingBroker.LogError(pollDependencyValidationException);

        return pollDependencyValidationException;
    }
    private PollDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var pollDependencyException =
            new PollDependencyException(exception);

        this.loggingBroker.LogError(pollDependencyException);

        return pollDependencyException;
    }
    private PollDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var pollDependencyException =
            new PollDependencyException(exception);

        this.loggingBroker.LogCritical(pollDependencyException);

        return pollDependencyException;
    }
    private PollServiceException CreateAndLogServiceException(Xeption exception)
    {
        var pollServiceException =
            new PollServiceException(exception);

        this.loggingBroker.LogError(pollServiceException);

        return pollServiceException;
    }
}