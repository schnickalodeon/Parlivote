using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Views.Motions;

namespace Parlivote.Web.Hubs;

public class VoteHub : Hub
{
    
    public const string AttendanceUpdatedMethod = "AttendanceUpdated";
    public const string VoteUpdatedMethod = "VoteUpdated";
    public const string VoteFinishedMethod = "VoteFinished";

    public async Task AttendanceUpdated(int attendanceCount)
    {
        await Clients.All.SendAsync(AttendanceUpdatedMethod, attendanceCount);
    }

    public async Task VoteUpdated(VoteView voteView)
    {
        await Clients.All.SendAsync(VoteUpdatedMethod, voteView);
    }

    public async Task VoteFinished()
    {
        await Clients.All.SendAsync(VoteFinishedMethod);
    }
}