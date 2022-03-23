using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Polls;

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

        this.apiBrokerMock.Verify(broker =>
            broker.PostPollAsync(It.IsAny<Poll>()),
            Times.Never);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock, 
            expectedPollValidationException);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
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

        this.apiBrokerMock.Verify(broker =>
            broker.PostPollAsync(It.IsAny<Poll>()),
            Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
    }

}