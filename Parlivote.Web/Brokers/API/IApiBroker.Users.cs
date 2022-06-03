using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Meetings;

namespace Parlivote.Web.Brokers.API;

public partial interface IApiBroker
{
    Task<List<User>>GetAllUsersAsync();
    Task<List<User>> GetAttendantUsersAsync();
    Task<List<User>> GetApplicantsAsync();
    Task<User> GetUserByIdAsync(Guid userId);
    Task<User> GetUserUntrackedByIdAsync(Guid userId);
    Task<User> PutUserAsync(User user);
}