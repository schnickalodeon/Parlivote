using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Moq;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Services.Foundations.Polls;
using Parlivote.Web.Services.Views.Polls;

namespace Parlivote.Web.Tests.Unit.Services.Views.Polls;

public partial class PollViewServiceTests
{
    private readonly Mock<IPollService> pollServiceMock;
    private readonly IPollViewService pollViewService;

    public PollViewServiceTests()
    {
        this.pollServiceMock = new Mock<IPollService>();

        this.pollViewService = new PollViewService(
            this.pollServiceMock.Object);
    }

    private static List<dynamic> CreateRandomPollViewCollections()
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