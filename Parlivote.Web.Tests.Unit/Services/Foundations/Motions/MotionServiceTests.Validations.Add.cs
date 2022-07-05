using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationException_OnAddIfMotionIsNullAndLogItAsync()
    {
        // Arrange
        Motion nullMotion = null;

        var nullMotionException =
            new NullMotionException();

        var expectedMotionValidationException =
            new MotionValidationException(nullMotionException);

        // Act
        Task<Motion> addMotionTask = this.motionService.AddAsync(nullMotion);
         
        // Assert
        await Assert.ThrowsAsync<MotionValidationException>(() => addMotionTask);

        this.apiBrokerMock.Verify(broker =>
            broker.PostMotionAsync(It.IsAny<Motion>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedMotionValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnAddIfMotionIsInvalidAndLogItAsync(string invalidText)
    {
        // Arrange
        var invalidMotion = new Motion
        {
            Id = Guid.Empty,
            ApplicantId = Guid.Empty,
            Title = invalidText,
            Text = invalidText,
        };

        var invalidMotionException =
            new InvalidMotionException();

        invalidMotionException.AddData(
            key: nameof(Motion.Id),
            values: ExceptionMessages.INVALID_ID);

        invalidMotionException.AddData(
            key: nameof(Motion.ApplicantId),
            values: ExceptionMessages.INVALID_ID);

        invalidMotionException.AddData(
            key: nameof(Motion.Title),
            values: ExceptionMessages.INVALID_STRING);

        invalidMotionException.AddData(
            key: nameof(Motion.Text),
            values: ExceptionMessages.INVALID_STRING);

        var expectedMotionValidationException
            = new MotionValidationException(invalidMotionException);

        // Act
        Task<Motion> addMotionTask = this.motionService.AddAsync(invalidMotion);

        // Assert
        await Assert.ThrowsAsync<MotionValidationException>(() => addMotionTask);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionValidationException);

        this.apiBrokerMock.Verify(broker =>
            broker.PostMotionAsync(It.IsAny<Motion>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
    }

}