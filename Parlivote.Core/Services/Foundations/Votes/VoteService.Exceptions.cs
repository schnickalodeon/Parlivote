using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using Xeptions;

namespace Parlivote.Core.Services.Foundations.Votes;

public partial class VoteService
{
    private delegate Task<Vote> ReturningVoteFunction();
    private delegate IQueryable<Vote> ReturningVotesFunction();

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
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            var lockedVoteException =
                new LockedVoteException(dbUpdateConcurrencyException);

            throw CreateAndLogDependencyValidationException(lockedVoteException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            var failedVoteStorageException =
                new FailedVoteStorageException(dbUpdateException);

            throw CreateAndLogDependencyException(failedVoteStorageException);
        }
        catch (DuplicateKeyException duplicateKeyException)
        {
            var alreadyExistsVoteException =
                new AlreadyExistsVoteException(duplicateKeyException);

            throw CreateAndLogDependencyValidationException(alreadyExistsVoteException);
        }
        catch (SqlException sqlException)
        {
            var failedVoteStorageException =
                new FailedVoteStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedVoteStorageException);
        }
        catch (Exception exception)
        {
            var failedVoteServiceException =
                new FailedVoteServiceException(exception);

            throw CreateAndLogServiceException(failedVoteServiceException);
        }
    }
    private IQueryable<Vote> TryCatch(ReturningVotesFunction returningVotesFunction)
    {
        try
        {
            return returningVotesFunction();
        }
        catch (SqlException sqlException)
        {
            var failedVoteStorageException =
                new FailedVoteStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedVoteStorageException);
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