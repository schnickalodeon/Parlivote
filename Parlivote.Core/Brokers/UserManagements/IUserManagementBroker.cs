using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Brokers.UserManagements;

public partial interface IUserManagementBroker
{
    Task<User> InsertUserAsync(User user, string password);
    IQueryable<User> SelectAllUsers();
    Task<User> SelectUserByIdAsync(string userId);
    Task<List<User>> SelectUserByRoleAsync(string role);
    Task<User> UpdateUserAsync(User user);
    Task<User> DeleteUserAsync(User user);
}