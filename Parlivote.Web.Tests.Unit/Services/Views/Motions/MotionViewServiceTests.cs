using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Moq;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Services.Foundations.Motions;
using Parlivote.Web.Services.Views.Motions;

namespace Parlivote.Web.Tests.Unit.Services.Views.Motions;

public partial class MotionViewServiceTests
{
    private readonly Mock<IMotionService> pollServiceMock;
    private readonly IMotionViewService pollViewService;

    public MotionViewServiceTests()
    {
        this.pollServiceMock = new Mock<IMotionService>();

        this.pollViewService = new MotionViewService(
            this.pollServiceMock.Object);
    }

    private static List<dynamic> CreateRandomMotionViewCollections()
    {
        int randomCount = Tests.GetRandomNumber();

        return Enumerable.Range(0, randomCount).Select(item =>
        {
            return new
            {
                Id = Guid.NewGuid(),
                AgendaItem = Tests.GetRandomString(),
                Text = Tests.GetRandomString()
            };
        }).ToList<dynamic>();
    }
}