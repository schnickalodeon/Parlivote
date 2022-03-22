using System.Threading.Tasks;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;

namespace Parlivote.Core.Services.Foundations.Polls;

public partial class PollService
{
    private void ValidatePoll(Poll poll)
    {
        ValidatePollIsNotNull(poll);
    }

    private void ValidatePollIsNotNull(Poll poll)
    {
        if (poll is null)
        {
            throw new NullPollException();
        }
    }
}