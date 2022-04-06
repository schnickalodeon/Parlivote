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

    public const string SetActiveMotionMethod = "ActiveMotionSet";

    public async Task ActiveMotionSet(MotionView activeMotion)
    {
        await this.motionViewService.SetActiveMotion(activeMotion);
        await Clients.All.SendAsync(SetActiveMotionMethod, activeMotion);
    }
}