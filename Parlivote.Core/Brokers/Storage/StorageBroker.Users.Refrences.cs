using Microsoft.EntityFrameworkCore;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    private void SetUserReferences(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(user => user.Motions)
            .WithOne(motion => motion.Applicant)
            .HasForeignKey(motion => motion.ApplicantId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}