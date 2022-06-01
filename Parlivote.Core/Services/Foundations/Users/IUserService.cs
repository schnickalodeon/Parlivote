using System;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Services.Foundations.Users;

public interface IUserService
{
    Task<User> RetrieveByIdAsync(Guid userId);
    Task<User> RetrieveUntrackedByIdAsync(Guid userId);
    IQueryable<User> RetrieveAll();
    Task<User> ModifyUserAsync(User user);
}