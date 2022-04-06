﻿using KellermanSoftware.CompareNetObjects;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Services.Foundations.Motions;
using Parlivote.Web.Services.Views.Motions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tynamix.ObjectFiller;

namespace Parlivote.Web.Tests.Unit.Services.Views.Motions;

public partial class MotionViewServiceTests
{
    private readonly Mock<IMotionService> motionServiceMock;
    private readonly IMotionViewService motionViewService;
    private readonly ICompareLogic compareLogic;

    public MotionViewServiceTests()
    {
        this.motionServiceMock = new Mock<IMotionService>();

        this.motionViewService = new MotionViewService(
            this.motionServiceMock.Object);

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
        return new
        {
            Id = Guid.NewGuid(),
            MeetingId = Guid.NewGuid(),
            Version = Tests.GetRandomNumber(),
            State = GetRandomState(),
            Text = Tests.GetRandomString()
        };
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