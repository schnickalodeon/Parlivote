using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Moq;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Services.Foundations.Users;
using RESTFulSense.Exceptions;
using Tynamix.ObjectFiller;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Users;

public partial class UsersServiceTests
{
    private readonly Mock<IApiBroker> apiBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly IUserService userService;

    public UsersServiceTests()
    {
        this.apiBrokerMock = new Mock<IApiBroker>();
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.userService = new UserService(
            this.apiBrokerMock.Object,
            this.loggingBrokerMock.Object
        );
    }

    private static User GetRandomUser()
    {
        return CreateUserFiller().Create();
    }

    private static List<User> GetRandomUsers()
    {
        return CreateUserFiller()
            .Create(count: Tests.GetRandomNumber())
            .ToList();
    }

    private static Filler<User> CreateUserFiller()
    {
        var filler = new Filler<User>();

        Guid randomUserId = Guid.NewGuid();
        string randomFirstName = new RealNames(NameStyle.FirstName).GetValue();
        string randomLastName = new RealNames(NameStyle.LastName).GetValue();

        filler.Setup()
            .OnType<DateTimeOffset?>().Use(new DateTimeRange(DateTime.Today).GetValue())
            .OnType<DateTimeOffset>().Use(new DateTimeRange(DateTime.Today).GetValue())
            .OnType<string>().Use(new MnemonicString(Tests.GetRandomNumber()))
            .OnProperty(user => user.Id).Use(randomUserId);

        return filler;
    }
    private static Dictionary<string, List<string>> CreateRandomDictionary()
    {
        var filler = new Filler<Dictionary<string, List<string>>>();

        return filler.Create();
    }

    public static TheoryData CriticalDependencyExceptions()
    {
        string exceptionMessage = Tests.GetRandomString();
        var responseMessage = new HttpResponseMessage();

        var httpRequestException =
            new HttpRequestException();

        var httpResponseUrlNotFoundException =
            new HttpResponseUrlNotFoundException(
                responseMessage: responseMessage,
                message: exceptionMessage);

        var httpResponseUnAuthorizedException =
            new HttpResponseUnauthorizedException(
                responseMessage: responseMessage,
                message: exceptionMessage);

        return new TheoryData<Exception>
        {
            httpRequestException,
            httpResponseUrlNotFoundException,
            httpResponseUnAuthorizedException
        };
    }
}
