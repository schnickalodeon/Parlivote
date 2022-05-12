using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Parlivote.Shared.Models.Votes;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Foundations.Votes;
using Parlivote.Web.Services.Views.Votes;
using Tynamix.ObjectFiller;

namespace Parlivote.Web.Tests.Unit.Services.Views.Votes;

public partial class VoteViewServiceTests
{
    private readonly Mock<IVoteService> voteServiceMock;
    private readonly IVoteViewService voteViewService;
    private readonly ICompareLogic compareLogic;

    public VoteViewServiceTests()
    {
        this.voteServiceMock = new Mock<IVoteService>();

        this.voteViewService = new VoteViewService(
            this.voteServiceMock.Object);

        var compareConfig = new ComparisonConfig();
        compareConfig.IgnoreProperty<Vote>(vote => vote.Id);
        this.compareLogic = new CompareLogic(compareConfig);
    }

    private Expression<Func<Vote, bool>> SameVoteAs(Vote expectedVote)
    {
        return actualVote => 
            this.compareLogic.Compare(expectedVote, actualVote).AreEqual;
    }

    private static dynamic CreateRandomVoteView()
    {
        return new
        {
            Id = Guid.NewGuid(),
            MotionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Value = new IntRange(0,2).GetValue()
        };
    }

    private static List<dynamic> CreateRandomVoteViewCollections()
    {
        int randomCount = Tests.GetRandomNumber();

        return Enumerable.Range(0, randomCount).Select(item =>
        {
            return new
            {
                Id = Guid.NewGuid(),
                MotionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Value = new IntRange(0, 2).GetValue()
            };

        }).ToList<dynamic>();
    }
}