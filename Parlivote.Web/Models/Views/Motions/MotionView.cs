using System;

namespace Parlivote.Web.Models.Views.Motions
{
    public class MotionView
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string State { get; set; }
        public string Text { get; set; }
    }
}
