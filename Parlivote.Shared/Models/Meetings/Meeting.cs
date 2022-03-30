using Parlivote.Shared.Models.Motions;

namespace Parlivote.Shared.Models.Meetings;

public class Meeting
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTimeOffset Start { get; set; }
    private ICollection<Motion> Motions { get; set; }
}