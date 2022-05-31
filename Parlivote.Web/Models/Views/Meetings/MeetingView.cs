using System;
using System.Collections.Generic;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Web.Models.Views.Motions;

namespace Parlivote.Web.Models.Views.Meetings
{
    public class MeetingView
    {
        public Guid? Id { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Start { get; set; } = DateTime.Today.AddHours(18);
        public List<MotionView> Motions { get; set; } = new();
    }
}
