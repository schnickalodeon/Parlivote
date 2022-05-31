using System;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Brokers.Storage;

public partial interface IStorageBroker
{
    public Task<User> SelectUserUntrackedByIdAsync(Guid userId);
    public Task<User> UpdateUserAsync(User user);
}