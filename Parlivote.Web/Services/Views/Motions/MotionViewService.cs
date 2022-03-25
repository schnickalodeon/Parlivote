using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Foundations.Motions;

namespace Parlivote.Web.Services.Views.Motions;

public class MotionViewService : IMotionViewService
{
    private readonly IMotionService pollService;
    public MotionViewService(IMotionService pollService)
    {
        this.pollService = pollService;
    }
    public async Task<MotionView> AddAsync(MotionView pollView)
    {
        Motion mappedMotion = 
            MapToMotion(pollView);

        Motion storageMotion = 
            await this.pollService.AddAsync(mappedMotion);

        MotionView mappedMotionView = 
            MapToMotionView(storageMotion);

        return mappedMotionView;
    }

    public async Task<List<MotionView>> GetAllAsync()
    {
        List<Motion> polls = 
            await this.pollService.RetrieveAllAsync();

        return polls.Select(AsMotionView).ToList();
    }

    private static Motion MapToMotion(MotionView pollView)
    {
        return new Motion
        {
            Id = pollView.Id,
            Version = pollView.Version,
            State = MotionStateConverter.FromString(pollView.State),
            Text = pollView.Text
        };
    }

    private static Func<Motion, MotionView> AsMotionView => MapToMotionView;
    private static MotionView MapToMotionView(Motion poll)
    {
        return new MotionView
        {
            Id = poll.Id,
            Version = poll.Version,
            State = poll.State.GetValue(),
            Text = poll.Text
        };
    }
}