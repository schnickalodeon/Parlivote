using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Shared.Models.Meetings;

public class Meeting
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTimeOffset Start { get; set; }
    public List<Motion>? Motions { get; set; }
}