using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Core.Services.Foundations.Polls;

public partial class PollService : IPollService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IStorageBroker storageBroker;

    public PollService(ILoggingBroker loggingBroker, IStorageBroker storageBroker)
    {
        this.loggingBroker = loggingBroker;
        this.storageBroker = storageBroker;
    }

    public Task<Poll> AddAsync(Poll poll) =>
        TryCatch(async () =>
        {
            ValidatePoll(poll);
            return await this.storageBroker.InsertPollAsync(poll);
        });

    public IQueryable<Poll> RetrieveAll()
    {
        throw new System.NotImplementedException();
    }
}