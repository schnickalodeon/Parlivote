using Microsoft.EntityFrameworkCore;
using Parlivote.Shared.Models.Votes;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    private static void SetVoteEntityConfiguration(ModelBuilder builder)
    {
        builder.Entity<Vote>().HasKey(vote => vote.Id);
        builder.Entity<Vote>().HasAlternateKey(vote => new {vote.MotionId, vote.UserId});
    }
}