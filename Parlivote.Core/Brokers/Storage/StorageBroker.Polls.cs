using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parlivote.Shared.Models.Polls;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    public DbSet<Poll> Polls { get; set; }
    public async Task<Poll> InsertPollAsync(Poll poll)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Poll> addedEntityEntry =
            await broker.Polls.AddAsync(poll);

        await broker.SaveChangesAsync();

        return addedEntityEntry.Entity;
    }

    public IQueryable<Poll> SelectAllPolls()
    {
        using var broker = new StorageBroker(this.configuration);
        return broker.Polls;
    }

    public async Task<Poll> SelectPollById(Guid pollId)
    {
        await using var broker = new StorageBroker(this.configuration);
        return await broker.Polls.FindAsync(pollId);
    }

    public async Task<Poll> UpdatePollAsync(Poll poll)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Poll> updatedEntityEntry =
            broker.Polls.Update(poll);

        await broker.SaveChangesAsync();

        return updatedEntityEntry.Entity;
    }

    public async Task<Poll> DeletePollAsync(Poll poll)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Poll> deletedEntityEntry =
            broker.Polls.Remove(poll);

        await broker.SaveChangesAsync();

        return deletedEntityEntry.Entity;
    }
}