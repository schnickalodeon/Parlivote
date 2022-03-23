using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models.Polls;
using Parlivote.Shared.Models.Polls.Exceptions;
using RESTFulSense.Exceptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Polls
{
    public partial class PollServiceTests
    {
        [Fact]
        public async Task ShouldThrowAndLogDependencyValidationException_OnAddIf_BadRequestExceptionOccurs()
        {
            // Arrange
            Poll somePoll = GetRandomPoll();

            string randomMessage = Tests.GetRandomString();
            var responseMessage = new HttpResponseMessage();

            var badRequestException =
                new HttpResponseBadRequestException(
                    responseMessage: responseMessage,
                    message: randomMessage);

            var expectedPollDependencyValidationException =
                new PollDependencyValidationException(
                    badRequestException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostPollAsync(It.IsAny<Poll>()))
                    .ThrowsAsync(badRequestException);

            // Act
            Task<Poll> addPollTask =
                this.pollService.AddAsync(somePoll);

            // Assert
            await Assert.ThrowsAsync<PollDependencyValidationException>(() => addPollTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostPollAsync(somePoll),
                Times.Once);

            Tests.VerifyExceptionLogged(
                this.loggingBrokerMock,
                expectedPollDependencyValidationException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
