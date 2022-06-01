using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Parlivote.Core.Brokers.UserManagements;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Brokers.Storage;

public partial class StorageBroker
{
    public Task<User> SelectUserUntrackedByIdAsync(Guid userId)
    {
        var broker = new StorageBroker(this.configuration);
        return broker.Users.AsNoTracking().FirstAsync(user => user.Id == userId);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        try
        {
            var broker = new StorageBroker(this.configuration);
            broker.Attach(user).State = EntityState.Modified;
            var res = await broker.SaveChangesAsync();

            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}