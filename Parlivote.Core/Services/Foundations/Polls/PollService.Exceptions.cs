using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;
using Xeptions;

namespace Parlivote.Core.Services.Foundations.Polls;

public partial class PollService
{
    private delegate Task<Poll> ReturningPollFunction();

    private async Task<Poll> TryCatch(ReturningPollFunction returningPollFunction)
    {
        try
        {
            return await returningPollFunction();
        }
        catch (DuplicateKeyException duplicateKeyException)
        {
            var alreadyExistsPollException =
                new AlreadyExistsPollException(duplicateKeyException);

            throw CreateAndLogDependencyValidationException(alreadyExistsPollException);
        }
        catch (SqlException sqlException)
        {
            var failedPollStorageException =
                new FailedPollStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedPollStorageException);
        }
    }

    private PollDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var pollDependencyValidationException =
            new PollDependencyValidationException(exception);

        this.loggingBroker.LogError(pollDependencyValidationException);

        return pollDependencyValidationException;
    }

    private PollDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var pollDependencyException =
            new PollDependencyException(exception);

        this.loggingBroker.LogCritical(pollDependencyException);

        return pollDependencyException;
    }
}