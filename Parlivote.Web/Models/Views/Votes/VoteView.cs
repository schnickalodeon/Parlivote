using System;
using Parlivote.Shared.Models.VoteValues;

namespace Parlivote.Web.Models.Views.Votes;

public class VoteView
{
    public Guid VoteId { get; set; }
    public Guid MotionId { get; set; }
    public Guid UserId { get; set; }
    public VoteValue Value { get; set; }
}