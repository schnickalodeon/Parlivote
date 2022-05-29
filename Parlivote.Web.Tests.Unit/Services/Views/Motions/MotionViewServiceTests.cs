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
using Tynamix.ObjectFiller;

namespace Parlivote.Web.Tests.Unit.Services.Views.Motions;

public partial class MotionViewServiceTests
{
    private readonly Mock<IMotionService> motionServiceMock;
    private readonly Mock<IMeetingService> meetingServiceMock;
    private readonly IMotionViewService motionViewService;
    private readonly ICompareLogic compareLogic;

    public MotionViewServiceTests()
    {
        this.motionServiceMock = new Mock<IMotionService>();
        this.meetingServiceMock = new Mock<IMeetingService>();

        this.motionViewService = new MotionViewService(
            this.motionServiceMock.Object,
            this.meetingServiceMock.Object);

        var compareConfig = new ComparisonConfig();
        compareConfig.IgnoreProperty<Motion>(motion => motion.Id);
        this.compareLogic = new CompareLogic(compareConfig);
    }

    private static MotionState GetRandomState()
    {
        int motionStateCount =
            Enum.GetValues(typeof(MotionState)).Length;

        int randomStateValue =
            new IntRange(min: 0, max: motionStateCount - 1).GetValue();

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
            Version = Tests.GetRandomNumber(),
            State = GetRandomState(),
            Text = Tests.GetRandomString(),
            Meeting = randomMeeting,
        };
    }

    private static Meeting GetRandomMeeting()
    {
        List<User> attendants = GetAttendantsList();
        var randomMeeting = new Meeting()
        {
            Id = Guid.NewGuid(),
            Description = Tests.GetRandomString(),
        };

        return randomMeeting;
    }

    private static List<User> GetAttendantsList()
    {
        var someUserCount = Tests.GetRandomNumber();
        var attendants = new List<User>();
        for (int i = 0; i < someUserCount; i++)
        {
            attendants.Add(new User());
        }

        return attendants;
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