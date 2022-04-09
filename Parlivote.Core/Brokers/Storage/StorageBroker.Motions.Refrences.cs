using Microsoft.EntityFrameworkCore;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    private static void SetMeetingReference(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Motion>()
            .HasOne(motion => motion.Meeting)
            .WithMany(meeting => meeting.Motions)
            .HasForeignKey(motion => motion.MeetingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}