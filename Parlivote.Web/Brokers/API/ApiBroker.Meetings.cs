using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Web.Brokers.API;

public partial class ApiBroker
{
    private const string MeetingsRelativeUrl = "/api/v1/meetings";

    public async Task<Meeting> PostMeetingAsync(Meeting meeting) =>
        await this.PostAsync(MeetingsRelativeUrl, meeting);

    public async Task<List<Meeting>> GetAllMeetingsAsync() =>
        await this.GetAsync<List<Meeting>>(MeetingsRelativeUrl);

    public async Task<List<Meeting>> GetAllMeetingsWithMotionsAsync() =>
        await this.GetAsync<List<Meeting>>($"{MeetingsRelativeUrl}/WithMotions");

    public async Task<Meeting> GetMeetingById(Guid meetingId) =>
        await this.GetAsync<Meeting>($"{MeetingsRelativeUrl}/{meetingId}");

    public async Task<Meeting> PutMeetingAsync(Meeting meeting) =>
        await this.PutAsync(MeetingsRelativeUrl,meeting);

    public async Task<Meeting> DeleteMeetingById(Guid meetingId) =>
        await this.DeleteAsync<Meeting>($"{MeetingsRelativeUrl}/{meetingId}");
}