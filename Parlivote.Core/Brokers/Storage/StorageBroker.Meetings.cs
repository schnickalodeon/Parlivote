using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parlivote.Shared.Models.Meetings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Motions;

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
        try
        {
            await using var broker = new StorageBroker(this.configuration);

            Meeting dbMeeting = await broker.Meetings
                .Include(m => m.Motions)
                .Include(m => m.AttendantUsers)
                .FirstAsync(dbMeeting => dbMeeting.Id == meeting.Id);

            dbMeeting.Description = meeting.Description;
            dbMeeting.Start = meeting.Start;

            List<Guid> motionIdsInDb = dbMeeting.Motions.Select(motion => motion.Id).ToList();
            List<Guid> motionIdsToAdd = meeting.Motions.Select(motion => motion.Id).Except(motionIdsInDb).ToList();
            List<Motion> motionsToAdd = await broker.Motions.Where(motion => motionIdsToAdd.Contains(motion.Id)).ToListAsync();
            dbMeeting.Motions.AddRange(motionsToAdd);

            List<Guid> attendantUserIdsInDb = dbMeeting.AttendantUsers.Select(user => user.Id).ToList();
            List<Guid> attendantsUserIdsToAdd = meeting.AttendantUsers.Select(user => user.Id).Except(attendantUserIdsInDb).ToList();
            List<User> attendancesToAdd = await broker.Users.Where(dbUser => attendantsUserIdsToAdd.Contains(dbUser.Id)).ToListAsync();
            List<User> attendantsToDelete = dbMeeting.AttendantUsers.Where(dbu => !meeting.AttendantUsers.Select(u => u.Id).Contains(dbu.Id)).ToList();

            dbMeeting.AttendantUsers.AddRange(attendancesToAdd);
            attendantsToDelete.ForEach(deleted => dbMeeting.AttendantUsers.Remove(deleted));

            EntityEntry<Meeting> updatedEntityEntry =
                broker.Meetings.Update(dbMeeting);

            await broker.SaveChangesAsync();

            return updatedEntityEntry.Entity;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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