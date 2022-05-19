using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    private void AddMeetingAttendanceReference(ModelBuilder builder)
    {
        builder.Entity<Meeting>()
            .HasMany<User>(user => user.AttendantUsers)
            .WithMany(meeting => meeting.Meetings)
            .UsingEntity(join => join.ToTable("Attendances"));
    }
}