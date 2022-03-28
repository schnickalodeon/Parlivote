using System;

namespace Parlivote.Web.Models.Views.Meetings
{
    public class MeetingView
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Start { get; set; }
    }
}
