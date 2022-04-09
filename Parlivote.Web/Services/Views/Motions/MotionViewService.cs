using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Foundations.Motions;

namespace Parlivote.Web.Services.Views.Motions;

public class MotionViewService : IMotionViewService
{
    private readonly IMotionService motionService;
    private readonly IMeetingService meetingService;
    public MotionViewService(
        IMotionService motionService, 
        IMeetingService meetingService)
    {
        this.motionService = motionService;
        this.meetingService = meetingService;
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
            await MapToMotionView(storageMotion);

        return mappedMotionView;
    }

    public async Task<List<MotionView>> GetAllAsync()
    {
        List<Motion> motions = 
            await this.motionService.RetrieveAllAsync();

        return await MapToMotionViews(motions);
    }

    public async Task<List<MotionView>> GetAllWithMeetingAsync()
    {
        List<Motion> motions =
            await this.motionService.RetrieveAllAsync();

        List<MotionView> motionViews =
            await MapToMotionViews(motions);

        return motionViews;
    }

    public async Task<MotionView> GetActiveAsync()
    {
        Motion activeMotion =
            await this.motionService.RetrieveActiveAsync();

        if (activeMotion is null)
            return null;

        return await MapToMotionView(activeMotion);
    }

    public async Task<MotionView> UpdateAsync(MotionView motionView)
    {
        Motion mappedMotion = MapToMotion(motionView);

        Motion updatedMotion =
            await this.motionService.ModifyAsync(mappedMotion);

        return await MapToMotionView(updatedMotion);
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

    private Func<Motion, Task<MotionView>> AsMotionView => MapToMotionView;

    private async Task<List<MotionView>> MapToMotionViews(List<Motion> motions)
    {
        var motionViews = new List<MotionView>();
        foreach (Motion motion in motions)
        {
            MotionView motionView = await MapToMotionView(motion);
            motionViews.Add(motionView);
        }

        return motionViews;
    }
    private async Task<MotionView> MapToMotionView(Motion motion)
    {
        string meetingName = "";

        if (motion.MeetingId.HasValue)
        {
            Meeting meeting =
                await this.meetingService.RetrieveByIdAsync(motion.MeetingId.Value);

            meetingName = meeting?.Description ?? "";
        }

        return new MotionView
        {
            MotionId = motion.Id,
            Version = motion.Version,
            MeetingId = motion.MeetingId,
            State = motion.State.GetValue(),
            Text = motion.Text,
            MeetingName = meetingName
        };
    }
}