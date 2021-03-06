using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Web.Brokers.API;

public partial class ApiBroker
{
    private const string UsersRelativeUrl = "/api/v1/users";
    public async Task<List<User>> GetAllUsersAsync() =>
        await this.GetAsync<List<User>>(UsersRelativeUrl);

    public async Task<List<User>> GetAttendantUsersAsync() =>
        await this.GetAsync<List<User>>($"{UsersRelativeUrl}/attendant");

    public async Task<List<User>> GetApplicantsAsync() =>
        await this.GetAsync<List<User>>($"{UsersRelativeUrl}/applicants");

    public async Task<User> GetUserByIdAsync(Guid userId) =>
        await this.GetAsync<User>($"{UsersRelativeUrl}/{userId}");

    public async Task<User> GetUserUntrackedByIdAsync(Guid userId) =>
        await this.GetAsync<User>($"{UsersRelativeUrl}/untracked/{userId}");

    public async Task<User> PutUserAsync(User user) =>
        await this.PutAsync(UsersRelativeUrl, user);
}