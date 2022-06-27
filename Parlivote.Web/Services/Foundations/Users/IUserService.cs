using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Web.Services.Foundations.Users;

public interface IUserService
{
    Task<List<User>> RetrieveAllAsync();
    Task<List<User>> RetrieveAttendantAsync();
    Task<User> RetrieveByIdAsync(Guid userId);
    Task<User> RetrieveByIdUntrackedAsync(Guid userId);
    Task<User> ModifyUserAsync(User user);
    Task<List<User>> RetrieveApplicantsAsync();
}