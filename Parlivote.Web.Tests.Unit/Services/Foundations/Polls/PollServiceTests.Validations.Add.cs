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
}