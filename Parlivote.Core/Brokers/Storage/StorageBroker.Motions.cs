using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parlivote.Shared.Models.Motions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    public DbSet<Motion> Motions { get; set; }
    public async Task<Motion> InsertMotionAsync(Motion poll)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Motion> addedEntityEntry =
            await broker.Motions.AddAsync(poll);

        await broker.SaveChangesAsync();

        return addedEntityEntry.Entity;
    }
    public IQueryable<Motion> SelectAllMotions()
    {
        using var broker = new StorageBroker(this.configuration);
        return broker.Motions;
    }
    public async Task<Motion> SelectMotionById(Guid pollId)
    {
        await using var broker = new StorageBroker(this.configuration);
        return await broker.Motions.FindAsync(pollId);
    }
    public async Task<Motion> UpdateMotionAsync(Motion poll)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Motion> updatedEntityEntry =
            broker.Motions.Update(poll);

        await broker.SaveChangesAsync();

        return updatedEntityEntry.Entity;
    }
    public async Task<Motion> DeleteMotionAsync(Motion poll)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Motion> deletedEntityEntry =
            broker.Motions.Remove(poll);

        await broker.SaveChangesAsync();

        return deletedEntityEntry.Entity;
    }
}