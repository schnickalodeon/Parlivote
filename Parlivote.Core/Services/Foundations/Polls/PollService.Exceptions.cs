using System;
using System.Threading.Tasks;
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
        catch (SqlException sqlException)
        {
            var failedPollStorageException =
                new FailedPollStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedPollStorageException);
        }
    }

    private PollDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var pollDependencyException =
            new PollDependencyException(exception);

        this.loggingBroker.LogCritical(pollDependencyException);

        return pollDependencyException;
    }
}