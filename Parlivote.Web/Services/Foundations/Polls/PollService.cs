using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;

namespace Parlivote.Web.Services.Foundations.Polls;

public partial class PollService : IPollService
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IApiBroker apiBroker;

    public PollService(ILoggingBroker loggingBroker, IApiBroker apiBroker)
    {
        this.loggingBroker = loggingBroker;
        this.apiBroker = apiBroker;
    }

    public Task<Poll> AddAsync(Poll poll) =>
        TryCatch(async () => 
        {
            ValidatePoll(poll);
            return await this.apiBroker.PostPollAsync(poll);
        });
}