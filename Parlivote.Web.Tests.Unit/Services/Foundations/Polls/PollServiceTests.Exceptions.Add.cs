﻿using System;
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
        public static TheoryData DependencyValidationException()
        {
            string randomMessage = Tests.GetRandomString();
            var responseMessage = new HttpResponseMessage();

            var badRequestException =
                new HttpResponseBadRequestException(
                    responseMessage: responseMessage,
                    message: randomMessage);

            var conflictException =
                new HttpResponseConflictException(
                    responseMessage: responseMessage,
                    message: randomMessage);

            return new TheoryData<Xeption>
            {
                badRequestException,
                conflictException
            };
        }

        [Theory]
        [MemberData(nameof(DependencyValidationException))]
        public async Task ShouldThrowAndLogDependencyValidationException_OnAdd_IfDependencyValidationExceptionOccurs(
            Xeption dependencyValidationException)
        {
            // Arrange
            Poll somePoll = GetRandomPoll();

            var expectedPollDependencyValidationException =
                new PollDependencyValidationException(
                    dependencyValidationException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostPollAsync(It.IsAny<Poll>()))
                    .ThrowsAsync(dependencyValidationException);

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

        public static TheoryData CriticalDependencyException()
        {
            string randomMessage = Tests.GetRandomString();
            var responseMessage = new HttpResponseMessage();

            var urlNotFounException =
                new HttpResponseUrlNotFoundException(
                    responseMessage: responseMessage,
                    message: randomMessage);

            var unauthorizedException =
                new HttpResponseUnauthorizedException(
                    responseMessage: responseMessage,
                    message: randomMessage);

            return new TheoryData<Xeption>
            {
                urlNotFounException,
                unauthorizedException
            };
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

        public static TheoryData DependencyException()
        {
            string randomMessage = Tests.GetRandomString();
            var responseMessage = new HttpResponseMessage();

            var httpResponseException =
                new HttpResponseException(
                    httpResponseMessage: responseMessage,
                    message: randomMessage);

            var httpResponseInternalServerErrorException =
                new HttpResponseInternalServerErrorException(
                    responseMessage: responseMessage,
                    message: randomMessage);

            return new TheoryData<Xeption>
            {
                httpResponseException,
                httpResponseInternalServerErrorException
            };
        }

        [Theory]
        [MemberData(nameof(DependencyException))]
        public async Task ShouldThrowAndLogDependencyException_OnAdd_IfDependencyExceptionErrorOccurs(
            Xeption dependencyException)
        {
            // Arrange
            Poll somePoll = GetRandomPoll();

            var expectedPollDependencyException =
                new PollDependencyException(
                    dependencyException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostPollAsync(It.IsAny<Poll>()))
                    .ThrowsAsync(dependencyException);

            // Act
            Task<Poll> addPollTask =
                this.pollService.AddAsync(somePoll);

            // Assert
            await Assert.ThrowsAsync<PollDependencyException>(() => addPollTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostPollAsync(somePoll),
                Times.Once);

            Tests.VerifyExceptionLogged(
                this.loggingBrokerMock,
                expectedPollDependencyException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // Arrange
            Poll somePoll = GetRandomPoll();
            string randomExceptionMessage = Tests.GetRandomString();
            var serviceException = new Exception(randomExceptionMessage);

            var failedPollServiceException =
                new FailedPollServiceException(serviceException);

            var expectedPollServiceException =
                new PollServiceException(failedPollServiceException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostPollAsync(It.IsAny<Poll>()))
                    .ThrowsAsync(serviceException);

            //Act
            Task<Poll> addPollTask = this.pollService.AddAsync(somePoll);

            //Assert
            await Assert.ThrowsAsync<PollServiceException>(() => addPollTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostPollAsync(It.IsAny<Poll>()),
                Times.Once);

            Tests.VerifyExceptionLogged(this.loggingBrokerMock, expectedPollServiceException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
