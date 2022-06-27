using KellermanSoftware.CompareNetObjects;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Services.Foundations.Motions;
using Parlivote.Web.Services.Views.Motions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Foundations.Users;
using Parlivote.Web.Services.Foundations.Votes;
using Tynamix.ObjectFiller;

namespace Parlivote.Web.Tests.Unit.Services.Views.Motions;

public partial class MotionViewServiceTests
{
    private readonly Mock<IMotionService> motionServiceMock;
    private readonly Mock<IMeetingService> meetingServiceMock;
    private readonly Mock<IUserService> userServiceMock;
    private readonly Mock<IVoteService> voteServiceMock;
    private readonly IMotionViewService motionViewService;
    private readonly ICompareLogic compareLogic;

    public MotionViewServiceTests()
    {
        this.motionServiceMock = new Mock<IMotionService>();
        this.meetingServiceMock = new Mock<IMeetingService>();
        this.userServiceMock = new Mock<IUserService>();
        this.voteServiceMock = new Mock<IVoteService>();

        this.motionViewService = new MotionViewService(
            this.motionServiceMock.Object,
            this.meetingServiceMock.Object,
            this.userServiceMock.Object,
            this.voteServiceMock.Object);

        var compareConfig = new ComparisonConfig();
        compareConfig.IgnoreProperty<Motion>(motion => motion.Id);
        this.compareLogic = new CompareLogic(compareConfig);
    }

    private static MotionState GetRandomState()
    {
        int randomStateValue = -1;
        do
        {
            int motionStateCount =
                Enum.GetValues(typeof(MotionState)).Length;

            randomStateValue =
                new IntRange(min: 0, max: motionStateCount - 1).GetValue();

        } while (randomStateValue == 1);
        
        return (MotionState)randomStateValue;
    }

    private Expression<Func<Motion, bool>> SameMotionAs(Motion expectedMotion)
    {
        return actualMeeting =>
            this.compareLogic.Compare(expectedMotion, actualMeeting).AreEqual;
    }

    private static dynamic CreateRandomMotionView()
    {
        Meeting randomMeeting = GetRandomMeeting();

        return new
        {
            Id = Guid.NewGuid(),
            MeetingId = randomMeeting.Id,
            Version = 0,
            State = GetRandomState(),
            Text = Tests.GetRandomString(),
            AppicantName = Tests.GetRandomString(),
            ApplicantId = Guid.NewGuid(),
            Meeting = randomMeeting,
        };
    }

    private static Meeting GetRandomMeeting()
    {
        var randomMeeting = new Meeting()
        {
            Id = Guid.NewGuid(),
            Description = Tests.GetRandomString(),
        };

        return randomMeeting;
    }

    private static List<dynamic> CreateRandomMotionViewCollections()
    {
        int randomCount = Tests.GetRandomNumber();

        var test = Enumerable.Range(0, randomCount).Select(item =>
        {
            return new
            {
                Id = Guid.NewGuid(),
                MeetingId = Guid.NewGuid(),
                Version = Tests.GetRandomNumber(),
                State = GetRandomState(),
                Text = Tests.GetRandomString()
            };
        }).ToList<dynamic>();

        return test;
    }
}