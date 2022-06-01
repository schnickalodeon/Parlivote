using System;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Brokers.UserManagements;
using Parlivote.Shared.Models.Identity;
using Parlivote.Shared.Models.Identity.Users;

namespace Parlivote.Core.Services.Foundations.Users;
public partial class UserService : IUserService
{
    private readonly IUserManagementBroker userManagementBroker;
    private readonly IStorageBroker storageBroker;
    private readonly ILoggingBroker loggingBroker;

    public UserService(
        IUserManagementBroker userManagementBroker,
        ILoggingBroker loggingBroker, 
        IStorageBroker storageBroker)
    {
        this.userManagementBroker = userManagementBroker;
        this.loggingBroker = loggingBroker;
        this.storageBroker = storageBroker;
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

    public Task<User> RetrieveUntrackedByIdAsync(Guid userId) =>
        TryCatch(async () =>
        {
            ValidateUserId(userId);

            User maybeUser =
                await this.storageBroker.SelectUserUntrackedByIdAsync(userId);

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
                await this.storageBroker.UpdateUserAsync(user);

            return updatedUser;
        });
}