using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Web.Services.Foundations.Users;

namespace Parlivote.Web.Services.Views.Users;

public class UserViewService : IUserViewService
{
    private readonly IUserService userService;

    public UserViewService(IUserService userService)
    {
        this.userService = userService;
    }

    public async Task<int> UpdateAttendance(Guid userId, bool attendant)
    {
        User userToUpdate =
            await this.userService.RetrieveByIdUntrackedAsync(userId);

        userToUpdate.IsAttendant = attendant;

        await this.userService.ModifyUserAsync(userToUpdate);

        List<User> users = await this.userService.RetrieveAllAsync();

        int attendantCount = users.Count(user => user.IsAttendant && user.IsLoggedIn);
        return attendantCount;
    }

    public async Task<bool> IsAttendant(Guid userId)
    {
        User user = await this.userService.RetrieveByIdAsync(userId);
        return (user.IsAttendant && user.IsLoggedIn);
    }
}