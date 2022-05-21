using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Foundations.Users;
using Parlivote.Web.Services.Views.Meetings;
using Tynamix.ObjectFiller;

namespace Parlivote.Web.Tests.Unit.Services.Views.Meetings;

public partial class MeetingViewServiceTests
{
    private readonly Mock<IMeetingService> meetingServiceMock;
    private readonly Mock<IUserService> userServiceMock;
    private readonly IMeetingViewService meetingViewService;
    private readonly ICompareLogic compareLogic;

    public MeetingViewServiceTests()
    {
        this.meetingServiceMock = new Mock<IMeetingService>();
        this.userServiceMock = new Mock<IUserService>();

        this.meetingViewService = new MeetingViewService(
            this.meetingServiceMock.Object,
            this.userServiceMock.Object);

        var compareConfig = new ComparisonConfig();
        compareConfig.IgnoreProperty<Meeting>(meeting => meeting.Id);
        this.compareLogic = new CompareLogic(compareConfig);
    }

    private Expression<Func<Meeting, bool>> SameMeetingAs(Meeting expectedMeeting)
    {
        return actualMeeting => 
            this.compareLogic.Compare(expectedMeeting, actualMeeting).AreEqual;
    }

    private static dynamic CreateRandomMeetingView()
    {
        List<User> attendants = GetAttendantsList();

        return new
        {
            Id = Guid.NewGuid(),
            Description = Tests.GetRandomString(),
            Start = Tests.GetRandomDateTimeOffset(),
            AttendantUsers = attendants,
            AttendantUsersCount = attendants.Count,
        };
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

    private static List<dynamic> CreateRandomMeetingViewCollections()
    {
        int randomCount = Tests.GetRandomNumber();

        return Enumerable.Range(0, randomCount).Select(item =>
        {
            List<User> attendants = GetAttendantsList();
            return new
            {
                Id = Guid.NewGuid(),
                Description = Tests.GetRandomString(),
                Start = Tests.GetRandomDateTimeOffset(),
                AttendantUsers = attendants,
                AttendantUsersCount = attendants.Count,
            };

        }).ToList<dynamic>();
    }
}