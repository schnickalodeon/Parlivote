using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Votes;
using Parlivote.Shared.Models.Votes;
using Tynamix.ObjectFiller;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Votes;

public partial class VoteServiceTests
{
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly Mock<IStorageBroker> storageBrokerMock;
    private readonly IVoteService voteService;

    public VoteServiceTests()
    {
        this.loggingBrokerMock = new Mock<ILoggingBroker>();
        this.storageBrokerMock = new Mock<IStorageBroker>();

        this.voteService = new VoteService(
            this.loggingBrokerMock.Object,
            this.storageBrokerMock.Object);
    }

    private static Vote GetRandomVote() =>
        GetVoteFiller().Create();

    private static IQueryable<Vote> GetRandomVotes()
    {
        return GetVoteFiller()
            .Create(count: Tests.GetRandomNumber())
            .AsQueryable();
    }

    private static Filler<Vote> GetVoteFiller()
    {
        var filler = new Filler<Vote>();

        var voteValue = new IntRange(0, 2);

        filler.Setup()
            .OnProperty(vote => vote.User).IgnoreIt()
            .OnProperty(vote => vote.Motion).IgnoreIt()
            .OnType<int>().Use(new IntRange(0,2));

        return filler;
    }

}