using System;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.UserManagements;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Services.Foundations.Users;
public partial class UserService : IUserService
{
    private readonly IUserManagementBroker userManagementBroker;
    private readonly ILoggingBroker loggingBroker;

    public UserService(
        IUserManagementBroker userManagementBroker,
        ILoggingBroker loggingBroker)
    {
        this.userManagementBroker = userManagementBroker;
        this.loggingBroker = loggingBroker;
    }

    public Task<User> RetrieveByIdAsync(Guid userId) =>
        TryCatch(async () =>
        {
            ValidateUserId(userId);

            User maybeUser =
                await this.userManagementBroker.SelectUserByIdAsync(userId);

            ValidateStorageUser(maybeUser, userId);

            return maybeUser;
        });

    public IQueryable<User> RetrieveAll() =>
        TryCatch(() => this.userManagementBroker.SelectAllUsers());

    public Task<User> ModifyUserAsync(User user) =>
        TryCatch(async () =>
        {
            ValidateUser(user);

            User maybeUser =
                await this.userManagementBroker.SelectUserByIdAsync(user.Id);

            ValidateStorageUser(maybeUser, user.Id);

            User updatedUser =
                await this.userManagementBroker.UpdateUserAsync(user);

            return updatedUser;
        });
}