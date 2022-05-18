using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Parlivote.Shared.Models.Identity;

namespace Parlivote.Core.Brokers.UserManagements
{
    public partial class UserManagementBroker : IUserManagementBroker
    {
        private readonly UserManager<User> userManagement;

        public UserManagementBroker(UserManager<User> userManagement)
        {
            this.userManagement = userManagement;
        }

        public IQueryable<User> SelectAllUsers() => this.userManagement.Users;

        public async Task<User> SelectUserByIdAsync(string userId)
        {
            var broker = new UserManagementBroker(this.userManagement);

            return await broker.userManagement.FindByIdAsync(userId);
        }

        public async Task<List<User>> SelectUserByRoleAsync(string role)
        {
            var broker = new UserManagementBroker(this.userManagement);

            IList<User> usersInRole = await broker.userManagement.GetUsersInRoleAsync(role);

            return usersInRole.ToList();
        }

        public async Task<User> InsertUserAsync(User user, string password)
        {
            var broker = new UserManagementBroker(this.userManagement);
            await broker.userManagement.CreateAsync(user, password);

            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var broker = new UserManagementBroker(this.userManagement);
            await broker.userManagement.UpdateAsync(user);

            return user;
        }

        public async Task<User> DeleteUserAsync(User user)
        {
            var broker = new UserManagementBroker(this.userManagement);
            await broker.userManagement.DeleteAsync(user);

            return user;
        }
    }
}