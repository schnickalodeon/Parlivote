using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parlivote.Shared.Models.Meetings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    public DbSet<Meeting> Meetings { get; set; }
    public async Task<Meeting> InsertMeetingAsync(Meeting meeting)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Meeting> addedEntityEntry =
            await broker.Meetings.AddAsync(meeting);

        await broker.SaveChangesAsync();

        return addedEntityEntry.Entity;
    }
    public IQueryable<Meeting> SelectAllMeetings()
    {
        using var broker = new StorageBroker(this.configuration);
        return broker.Meetings;
    }
    public async Task<Meeting> SelectMeetingById(Guid meetingId)
    {
        await using var broker = new StorageBroker(this.configuration);
        return await broker.Meetings.FindAsync(meetingId);
    }
    public async Task<Meeting> UpdateMeetingAsync(Meeting meeting)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Meeting> updatedEntityEntry =
            broker.Meetings.Update(meeting);

        await broker.SaveChangesAsync();

        return updatedEntityEntry.Entity;
    }
    public async Task<Meeting> DeleteMeetingAsync(Meeting meeting)
    {
        await using var broker = new StorageBroker(this.configuration);

        EntityEntry<Meeting> deletedEntityEntry =
            broker.Meetings.Remove(meeting);

        await broker.SaveChangesAsync();

        return deletedEntityEntry.Entity;
    }
}