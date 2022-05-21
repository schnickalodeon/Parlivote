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

    public async Task AttendanceUpdated(MeetingView meetingView)
    {
        await Clients.All.SendAsync(AttendanceUpdatedMethod, meetingView);
    }

    public async Task VoteUpdated(VoteView vote)
    {
        await Clients.All.SendAsync(VoteUpdatedMethod, vote);
    }
}