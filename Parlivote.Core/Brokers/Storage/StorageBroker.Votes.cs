using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parlivote.Shared.Models.Votes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    public DbSet<Vote> Votes { get; set; }
    public async Task<Vote> InsertVoteAsync(Vote vote)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Vote> addedEntityEntry =
            await broker.Votes.AddAsync(vote);

        await broker.SaveChangesAsync();

        return addedEntityEntry.Entity;
    }
    public IQueryable<Vote> SelectAllVotes()
    {
        using var broker = new StorageBroker(this.configuration);
        return broker.Votes;
    }
    public async Task<Vote> SelectVoteById(Guid voteId)
    {
        await using var broker = new StorageBroker(this.configuration);
        return await broker.Votes.FindAsync(voteId);
    }
    public async Task<Vote> UpdateVoteAsync(Vote vote)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Vote> updatedEntityEntry =
            broker.Votes.Update(vote);

        await broker.SaveChangesAsync();

        return updatedEntityEntry.Entity;
    }
    public async Task<Vote> DeleteVoteAsync(Vote vote)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Vote> deletedEntityEntry =
            broker.Votes.Remove(vote);

        await broker.SaveChangesAsync();

        return deletedEntityEntry.Entity;
    }
}