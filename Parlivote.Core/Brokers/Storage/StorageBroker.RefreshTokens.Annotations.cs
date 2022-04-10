using Microsoft.EntityFrameworkCore;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    private static void ApplyRefreshTokenAnnotations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>()
            .HasKey(entity => entity.Token);
    }
}