using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Moq;
using Parlivote.Shared.Models.Votes;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Services.Foundations.Votes;
using RESTFulSense.Exceptions;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly Mock<IApiBroker> apiBrokerMock;
    private readonly IVoteService voteService;
    public VoteServiceTests()
    {
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.apiBrokerMock = new Mock<IApiBroker>();

        this.voteService = new VoteService(
            this.loggingBrokerMock.Object,
            this.apiBrokerMock.Object);
    }
    private static Vote GetRandomVote() =>
        GetVoteFiller(dates: Tests.GetRandomDateTimeOffset()).Create();

    private static Filler<Vote> GetVoteFiller(DateTimeOffset dates)
    {
        var filler = new Filler<Vote>();

        filler.Setup()
            .OnType<DateTimeOffset>().Use(dates);

        return filler;
    }

    public static TheoryData DependencyException()
    {
        string randomMessage = Tests.GetRandomString();
        var responseMessage = new HttpResponseMessage();

        var httpResponseException =
            new HttpResponseException(
                httpResponseMessage: responseMessage,
                message: randomMessage);

        var httpResponseInternalServerErrorException =
            new HttpResponseInternalServerErrorException(
                responseMessage: responseMessage,
                message: randomMessage);

        return new TheoryData<Xeption>
        {
            httpResponseException,
            httpResponseInternalServerErrorException
        };
    }

    public static TheoryData CriticalDependencyException()
    {
        string randomMessage = Tests.GetRandomString();
        var responseMessage = new HttpResponseMessage();

        var urlNotFounException =
            new HttpResponseUrlNotFoundException(
                responseMessage: responseMessage,
                message: randomMessage);

        var unauthorizedException =
            new HttpResponseUnauthorizedException(
                responseMessage: responseMessage,
                message: randomMessage);

        return new TheoryData<Xeption>
        {
            urlNotFounException,
            unauthorizedException
        };
    }

    public static TheoryData DependencyValidationException()
    {
        string randomMessage = Tests.GetRandomString();
        var responseMessage = new HttpResponseMessage();

        var badRequestException =
            new HttpResponseBadRequestException(
                responseMessage: responseMessage,
                message: randomMessage);

        var conflictException =
            new HttpResponseConflictException(
                responseMessage: responseMessage,
                message: randomMessage);

        return new TheoryData<Xeption>
        {
            badRequestException,
            conflictException
        };
    }
}