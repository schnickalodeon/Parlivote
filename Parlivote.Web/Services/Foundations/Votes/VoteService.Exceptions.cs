using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;

namespace Parlivote.Web.Services.Foundations.Votes;

public partial class VoteService
{
    private delegate Task<Vote> ReturningVoteFunction();
    private delegate Task<List<Vote>> ReturningVotesFunction();

    private async Task<Vote> TryCatch(ReturningVoteFunction returningVoteFunction)
    {
        try
        {
            return await returningVoteFunction();
        }
        catch (NullVoteException nullVoteException)
        {
            throw CreateAndLogValidationException(nullVoteException);
        }
        catch (NotFoundVoteException notFoundVoteException)
        {
            throw CreateAndLogValidationException(notFoundVoteException);
        }
        catch (InvalidVoteException invalidVoteException)
        {
            throw CreateAndLogValidationException(invalidVoteException);
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
            var failedVoteServiceException =
                new FailedVoteServiceException(exception);

            throw CreateAndLogServiceException(failedVoteServiceException);
        }
    }
    private async Task<List<Vote>> TryCatch(ReturningVotesFunction returningVotesFunction)
    {
        try
        {
            return await returningVotesFunction();
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
            var failedVoteServiceException =
                new FailedVoteServiceException(exception);

            throw CreateAndLogServiceException(failedVoteServiceException);
        }
    }
    private VoteValidationException CreateAndLogValidationException(Xeption exception)
    {
        var voteValidationException =
            new VoteValidationException(exception);

        this.loggingBroker.LogError(voteValidationException);

        return voteValidationException;
    }
    private VoteDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var voteDependencyValidationException =
            new VoteDependencyValidationException(exception);

        this.loggingBroker.LogError(voteDependencyValidationException);

        return voteDependencyValidationException;
    }
    private VoteDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var voteDependencyException =
            new VoteDependencyException(exception);

        this.loggingBroker.LogError(voteDependencyException);

        return voteDependencyException;
    }
    private VoteDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var voteDependencyException =
            new VoteDependencyException(exception);

        this.loggingBroker.LogCritical(voteDependencyException);

        return voteDependencyException;
    }
    private VoteServiceException CreateAndLogServiceException(Xeption exception)
    {
        var voteServiceException =
            new VoteServiceException(exception);

        this.loggingBroker.LogError(voteServiceException);

        return voteServiceException;
    }
}