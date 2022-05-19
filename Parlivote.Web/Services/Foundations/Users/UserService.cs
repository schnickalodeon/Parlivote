using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;

namespace Parlivote.Web.Services.Foundations.Users;
public partial class UserService : IUserService
{
    private readonly IApiBroker apiBroker;
    private readonly ILoggingBroker loggingBroker;

    public UserService(
        IApiBroker apiBroker,
        ILoggingBroker loggingBroker)
    {
        this.apiBroker = apiBroker;
        this.loggingBroker = loggingBroker;
    }
    public Task<List<User>> RetrieveAllAsync() =>
        TryCatch(async () => await this.apiBroker.GetAllUsersAsync());
}