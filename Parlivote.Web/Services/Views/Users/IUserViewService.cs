using System;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Web.Services.Views.Users;

public interface IUserViewService
{
    Task<int> UpdateAttendance(Guid userId, bool attendant);
    Task<bool> IsAttendant(Guid userId);
}