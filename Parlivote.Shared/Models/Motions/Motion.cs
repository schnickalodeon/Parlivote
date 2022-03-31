using System.Text.Json.Serialization;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Shared.Models.Motions
{
    public class Motion
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public Guid? MeetingId { get; set; }
        [JsonIgnore]
        public Meeting? Meeting { get; set; }
        public MotionState State { get; set; }
        public string Text { get; set; }
    }
}