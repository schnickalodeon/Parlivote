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
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Polls
{
    public partial class PollServiceTests
    {
        public static TheoryData CriticalDependencyException()
        {
            string randomMessage = Tests.GetRandomString();
            var responseMessage = new HttpResponseMessage();

            var badRequestException =
                new HttpResponseUrlNotFoundException(
                    responseMessage: responseMessage,
                    message: randomMessage);

            var unauthorizedException =
                new HttpResponseUnauthorizedException(
                    responseMessage: responseMessage,
                    message: randomMessage);

            return new TheoryData<Xeption>
            {
                badRequestException,
                unauthorizedException
            };
        }

        [Fact]
        public async Task ShouldThrowAndLogDependencyValidationException_OnAdd_IfBadRequestExceptionOccurs()
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

        [Theory]
        [MemberData(nameof(CriticalDependencyException))]
        public async Task ShouldThrowAndLogCriticalDependencyException_OnAdd_IfCriticalExceptionOccurs(
            Xeption criticalException)
        {
            // Arrange
            Poll somePoll = GetRandomPoll();

            var expectedPollDependencyException =
                new PollDependencyException(
                    criticalException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostPollAsync(It.IsAny<Poll>()))
                    .ThrowsAsync(criticalException);

            // Act
            Task<Poll> addPollTask =
                this.pollService.AddAsync(somePoll);

            // Assert
            await Assert.ThrowsAsync<PollDependencyException>(() => addPollTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostPollAsync(somePoll),
                Times.Once);

            Tests.VerifyCriticalExceptionLogged(
                this.loggingBrokerMock,
                expectedPollDependencyException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        
    }
}
