using System;
using System.Collections.Generic;
using Parlivote.Web.Models.Views.Votes;

namespace Parlivote.Web.Models.Views.Motions
{
    public class MotionView
    {
        public Guid MotionId { get; set; }
        public string Text { get; set; }
        public string State { get; set; }
        public Guid? ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public Guid? MeetingId { get; set; }
        public string MeetingName { get; set; }
        public List<VoteView> VoteViews { get; set; }

        public MotionView Clone()
        {
            return new MotionView
            {
                MeetingId = this.MeetingId,
                MeetingName = this.MeetingName,
                MotionId = this.MotionId,
                Text = this.Text,
                State = this.State,
                VoteViews = new List<VoteView>(this.VoteViews),
            };
        }
    }
}
