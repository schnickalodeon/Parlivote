using System;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;

namespace Parlivote.Web.Services.Foundations.Polls;

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