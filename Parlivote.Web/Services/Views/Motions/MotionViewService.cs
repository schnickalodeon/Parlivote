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
    private readonly IMotionService motionService;
    public MotionViewService(IMotionService motionService)
    {
        this.motionService = motionService;
    }
    public async Task<MotionView> AddAsync(MotionView pollView)
    {
        var mappedMotion = new Motion
        {
            Id = Guid.NewGuid(),
            MeetingId = pollView.MeetingId,
            State = MotionState.Submitted,
            Text = pollView.Text,
            Version = pollView.Version
        };

            Motion storageMotion = 
            await this.motionService.AddAsync(mappedMotion);

        MotionView mappedMotionView = 
            MapToMotionView(storageMotion);

        return mappedMotionView;
    }

    public async Task<List<MotionView>> GetAllAsync()
    {
        List<Motion> polls = 
            await this.motionService.RetrieveAllAsync();

        return polls.Select(AsMotionView).ToList();
    }

    public async Task<MotionView> GetActiveAsync()
    {
        Motion activeMotion =
            await this.motionService.RetrieveActiveAsync();

        return MapToMotionView(activeMotion);
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