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

        if (activeMotion is null)
            return null;

        return MapToMotionView(activeMotion);
    }

    public async Task<MotionView> UpdateAsync(MotionView motionView)
    {
        Motion mappedMotion = MapToMotion(motionView);

        Motion updatedMotion =
            await this.motionService.ModifyAsync(mappedMotion);

        return MapToMotionView(updatedMotion);
    }

    public async Task<MotionView> SetState(MotionView motionView, MotionState state)
    {
        Motion mappedMotion = MapToMotion(motionView);

        mappedMotion.State = state;

        Motion updatedMotion =
            await this.motionService.ModifyAsync(mappedMotion);

        return MapToMotionView(updatedMotion);

    }

    public async Task<MotionView> SetActive(MotionView motionView)
    {
        MotionView activeMotion =
            await this.GetActiveAsync();

        if (activeMotion != null)
            return null;

        return await SetState(motionView, MotionState.Pending);
    }

    private static Motion MapToMotion(MotionView motionView)
    {
        return new Motion
        {
            Id = motionView.MotionId,
            MeetingId = motionView.MeetingId,
            Version = motionView.Version,
            State = MotionStateConverter.FromString(motionView.State),
            Text = motionView.Text
        };
    }

    private static Func<Motion, MotionView> AsMotionView => MapToMotionView;
    private static MotionView MapToMotionView(Motion motion)
    {
        return new MotionView
        {
            MotionId = motion.Id,
            Version = motion.Version,
            MeetingId = motion.MeetingId,
            State = motion.State.GetValue(),
            Text = motion.Text
        };
    }
}