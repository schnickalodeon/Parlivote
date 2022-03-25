using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Polls;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Polls;
using Parlivote.Web.Services.Foundations.Polls;

namespace Parlivote.Web.Services.Views.Polls;

public class PollViewService : IPollViewService
{
    private readonly IPollService pollService;
    public PollViewService(IPollService pollService)
    {
        this.pollService = pollService;
    }
    public async Task<PollView> AddAsync(PollView pollView)
    {
        Poll mappedPoll = 
            MapToPoll(pollView);

        Poll storagePoll = 
            await this.pollService.AddAsync(mappedPoll);

        PollView mappedPollView = 
            MapToPollView(storagePoll);

        return mappedPollView;
    }

    public async Task<List<PollView>> GetAllAsync()
    {
        List<Poll> polls = 
            await this.pollService.RetrieveAllAsync();

        return polls.Select(AsPollView).ToList();
    }

    private static Poll MapToPoll(PollView pollView)
    {
        return new Poll
        {
            Id = pollView.Id,
            AgendaItem = pollView.AgendaItem,
            Text = pollView.Text
        };
    }

    private static Func<Poll, PollView> AsPollView => MapToPollView;
    private static PollView MapToPollView(Poll poll)
    {
        return new PollView
        {
            Id = poll.Id,
            AgendaItem = poll.AgendaItem,
            Text = poll.Text
        };
    }
}