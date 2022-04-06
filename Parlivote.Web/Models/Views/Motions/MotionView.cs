using System;

namespace Parlivote.Web.Models.Views.Motions
{
    public class MotionView
    {
        public Guid MotionId { get; set; }
        public Guid? MeetingId { get; set; }
        public int Version { get; set; }
        public string State { get; set; }
        public string Text { get; set; }
    }
}
