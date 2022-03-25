namespace Parlivote.Shared.Models.Motions
{
    public class Motion
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public MotionState State { get; set; }
        public string Text { get; set; }
    }
}