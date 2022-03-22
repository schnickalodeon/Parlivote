using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Polls;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;
using Tynamix.ObjectFiller;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Polls;

public partial class PollServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationException_OnAddIfPollIsNullAndLogItAsync()
    {
        // Arrange
        Poll nullPoll = null;

        var nullPollException =
            new NullPollException();

        var expectedPollValidationException =
            new PollValidationException(nullPollException);

        // Act
        Task<Poll> addPollTask = this.pollService.AddAsync(nullPoll);
         
        // Assert
        await Assert.ThrowsAsync<PollValidationException>(() => addPollTask);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPollAsync(It.IsAny<Poll>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedPollValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnAddIfPollIsInvalidAndLogItAsync(string invalidText)
    {
        // Arrange
        var invalidPoll = new Poll
        {
            Id = Guid.Empty,
            Text = invalidText
        };

        var invalidPollException =
            new InvalidPollException();

        invalidPollException.AddData(
            key: nameof(Poll.Id),
            values: ExceptionMessages.INVALID_ID);

        invalidPollException.AddData(
            key: nameof(Poll.Text),
            values: ExceptionMessages.INVALID_STRING);

        var expectedPollValidationException
            = new PollValidationException(invalidPollException);

        // Act
        Task<Poll> addPollTask = this.pollService.AddAsync(invalidPoll);

        // Assert
        await Assert.ThrowsAsync<PollValidationException>(() => addPollTask);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedPollValidationException);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPollAsync(It.IsAny<Poll>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}