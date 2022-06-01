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
    public async Task<Motion> UpdateMotionAsync(Motion motion)
    {
        try
        {
            await using var broker = new StorageBroker(this.configuration);

            broker.Attach(motion).State = EntityState.Modified;

            await broker.SaveChangesAsync();

            return motion;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            throw exception;
        }
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