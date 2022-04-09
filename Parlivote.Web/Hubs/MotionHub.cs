using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Services.Views.Motions;

namespace Parlivote.Web.Hubs;

public class MotionHub : Hub
{
    private readonly IMotionViewService motionViewService;

    public MotionHub(IMotionViewService motionViewService)
    {
        this.motionViewService = motionViewService;
    }

    public const string SetStateMethod = "SetState";

    public async Task SetState(MotionView motionView)
    {
        await this.motionViewService.UpdateAsync(motionView);
        await Clients.All.SendAsync(SetStateMethod, motionView);
    }
}