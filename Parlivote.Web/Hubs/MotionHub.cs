using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Models.Views.Motions;

namespace Parlivote.Web.Hubs;

public class MotionHub : Hub
{
    public const string SetActiveMotionMethod = "ActiveMotionSet";

    public async Task ActiveMotionSet(MotionView activeMotion)
    {
        await Clients.All.SendAsync(SetActiveMotionMethod, activeMotion);
    }
}