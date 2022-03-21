using System.Threading.Tasks;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Core.Services.Foundations.Polls;

public class PollService : IPollService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IStorageBroker storageBroker;

    public PollService(ILoggingBroker loggingBroker, IStorageBroker storageBroker)
    {
        this.loggingBroker = loggingBroker;
        this.storageBroker = storageBroker;
    }
    public async Task<Poll> AddPollAsync(Poll poll)
    {
        return await this.storageBroker.InsertPollAsync(poll);
    }
}