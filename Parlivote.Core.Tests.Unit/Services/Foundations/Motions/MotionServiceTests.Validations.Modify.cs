using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Motions;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Motions;
using Parlivote.Shared.Models.Motions.Exceptions;
using Tynamix.ObjectFiller;
using Xunit;
using Xunit.Sdk;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Motions;

public partial class MotionServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationException_OnModifyIfMotionIsNullAndLogItAsync()
    {
        // Arrange
        Motion nullMotion = null;

        var nullMotionException =
            new NullMotionException();

        var expectedMotionValidationException =
            new MotionValidationException(nullMotionException);

        // Act
        Task<Motion> modifyMotionTask = this.motionService.ModifyAsync(nullMotion);
         
        // Assert
        await Assert.ThrowsAsync<MotionValidationException>(() => modifyMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMotionAsync(It.IsAny<Motion>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedMotionValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnModifyIfMotionIsInvalidAndLogItAsync(string invalidText)
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
            key: nameof(Motion.Title),
            values: ExceptionMessages.INVALID_STRING);

        invalidMotionException.AddData(
            key: nameof(Motion.Text),
            values: ExceptionMessages.INVALID_STRING);

        invalidMotionException.AddData(
            key: nameof(Motion.ApplicantId),
            values: ExceptionMessages.INVALID_ID);

        var expectedMotionValidationException
            = new MotionValidationException(invalidMotionException);

        // Act
        Task<Motion> modifyMotionTask = this.motionService.ModifyAsync(invalidMotion);

        // Assert
        await Assert.ThrowsAsync<MotionValidationException>(() => modifyMotionTask);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMotionValidationException);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertMotionAsync(It.IsAny<Motion>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfMotionDoesNotExistAndLogItAsync()
    {
        // given
        Motion randomMotion = GetRandomMotion();
        Motion nonExistMotion = randomMotion;
        Motion nullMotion = null;

        var notFoundMotionException =
            new NotFoundMotionException(nonExistMotion.Id);

        var expectedMotionValidationException =
            new MotionValidationException(notFoundMotionException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectMotionById(nonExistMotion.Id))
                .ReturnsAsync(nullMotion);

        // when 
        Task<Motion> modifyMotionTask =
            this.motionService.ModifyAsync(nonExistMotion);

        // then
        await Assert.ThrowsAsync<MotionValidationException>(() => modifyMotionTask);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectMotionById(nonExistMotion.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock,
            expectedMotionValidationException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}