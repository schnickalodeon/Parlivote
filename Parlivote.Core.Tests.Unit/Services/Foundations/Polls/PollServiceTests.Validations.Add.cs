using System.Threading.Tasks;
using Moq;
using Parlivote.Core.Brokers.Logging;
using Parlivote.Core.Brokers.Storage;
using Parlivote.Core.Services.Foundations.Polls;
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
}