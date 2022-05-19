using System;
using System.Linq;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.UserManagements;
using Parlivote.Core.Services.Foundations.Users;
using Parlivote.Shared.Models.Identity.Users;
using Tynamix.ObjectFiller;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Users;

public partial class UserServiceTests
{
    private readonly Mock<IUserManagementBroker> userManagementBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly IUserService userService;

    public UserServiceTests()
    {
        this.userManagementBrokerMock = new Mock<IUserManagementBroker>();
        this.loggingBrokerMock = new Mock<ILoggingBroker>();

        this.userService = new UserService(
            this.userManagementBrokerMock.Object,
            this.loggingBrokerMock.Object);
    }
    private static User GetRandomUser() =>
        CreateUserFiller().Create();

    private static IQueryable<User> GetRandomUsers() =>
        CreateUserFiller()
            .Create(count: Tests.GetRandomNumber())
                .AsQueryable();

    private static Filler<User> CreateUserFiller()
    {
        var userFiller = new Filler<User>();

        userFiller.Setup()
            .OnType<DateTimeOffset?>().Use(new DateTimeRange(DateTime.Today).GetValue())
            .OnType<DateTimeOffset>().Use(new DateTimeRange(DateTime.Today).GetValue())
            .OnType<string>().Use(new MnemonicString(Tests.GetRandomNumber()))
            .OnProperty(user => user.Id).Use(Guid.NewGuid());

        return userFiller;
    }
}