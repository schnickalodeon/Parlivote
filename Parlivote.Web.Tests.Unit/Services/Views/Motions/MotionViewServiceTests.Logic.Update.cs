using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Motions;
using Parlivote.Web.Models.Views.Motions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Views.Motions;

public partial class MotionViewServiceTests
{
    [Fact]
    public async Task ShouldUpdateMotionViewAsyncAsync()
    {
        // Arrange
        dynamic someMotionViewInput = CreateRandomMotionView();

        var someMotionView = new MotionView
        {
            MotionId = someMotionViewInput.Id,
            MeetingId = someMotionViewInput.MeetingId,
            State = ((MotionState)someMotionViewInput.State).GetValue(),
            Text = someMotionViewInput.Text,
            Version = someMotionViewInput.Version
        };

        MotionView inputMotionView = someMotionView;
        MotionView expectedMotionView = inputMotionView;

        var someMotion = new Motion
        {
            Id = someMotionViewInput.Id,
            MeetingId = someMotionViewInput.MeetingId,
            State = someMotionViewInput.State,
            Text = someMotionViewInput.Text,
            Version = someMotionViewInput.Version
        };

        Motion expectedInputMotion = someMotion;
        Motion returnedMotion = expectedInputMotion;

        this.motionServiceMock.Setup(service =>
            service.ModifyAsync(It.IsAny<Motion>()))
                .ReturnsAsync(returnedMotion);

        // Act
        MotionView actualMotionView =
            await this.motionViewService.UpdateAsync(inputMotionView);

        // Assert
        actualMotionView.Should().BeEquivalentTo(
            expectedMotionView, 
            options => options.Excluding(motionView => motionView.MotionId));

        this.motionServiceMock.Verify(service =>
            service.ModifyAsync(It.Is(SameMotionAs(expectedInputMotion))),
            Times.Once);
    }
}