using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Meetings
{
    public partial class MeetingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationException))]
        public async Task ShouldThrowAndLogDependencyValidationException_OnAdd_IfDependencyValidationExceptionOccurs(
            Xeption dependencyValidationException)
        {
            // Arrange
            Meeting someMeeting = GetRandomMeeting();

            var expectedMeetingDependencyValidationException =
                new MeetingDependencyValidationException(
                    dependencyValidationException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostMeetingAsync(It.IsAny<Meeting>()))
                    .ThrowsAsync(dependencyValidationException);

            // Act
            Task<Meeting> addMeetingTask =
                this.meetingService.AddAsync(someMeeting);

            // Assert
            await Assert.ThrowsAsync<MeetingDependencyValidationException>(() => addMeetingTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostMeetingAsync(someMeeting),
                Times.Once);

            Tests.VerifyExceptionLogged(
                this.loggingBrokerMock,
                expectedMeetingDependencyValidationException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(CriticalDependencyException))]
        public async Task ShouldThrowAndLogCriticalDependencyException_OnAdd_IfCriticalExceptionOccurs(
            Xeption criticalException)
        {
            // Arrange
            Meeting someMeeting = GetRandomMeeting();

            var expectedMeetingDependencyException =
                new MeetingDependencyException(
                    criticalException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostMeetingAsync(It.IsAny<Meeting>()))
                    .ThrowsAsync(criticalException);

            // Act
            Task<Meeting> addMeetingTask =
                this.meetingService.AddAsync(someMeeting);

            // Assert
            await Assert.ThrowsAsync<MeetingDependencyException>(() => addMeetingTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostMeetingAsync(someMeeting),
                Times.Once);

            Tests.VerifyCriticalExceptionLogged(
                this.loggingBrokerMock,
                expectedMeetingDependencyException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        [Theory]
        [MemberData(nameof(DependencyException))]
        public async Task ShouldThrowAndLogDependencyException_OnAdd_IfDependencyExceptionErrorOccurs(
            Xeption dependencyException)
        {
            // Arrange
            Meeting someMeeting = GetRandomMeeting();

            var expectedMeetingDependencyException =
                new MeetingDependencyException(
                    dependencyException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostMeetingAsync(It.IsAny<Meeting>()))
                    .ThrowsAsync(dependencyException);

            // Act
            Task<Meeting> addMeetingTask =
                this.meetingService.AddAsync(someMeeting);

            // Assert
            await Assert.ThrowsAsync<MeetingDependencyException>(() => addMeetingTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostMeetingAsync(someMeeting),
                Times.Once);

            Tests.VerifyExceptionLogged(
                this.loggingBrokerMock,
                expectedMeetingDependencyException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // Arrange
            Meeting someMeeting = GetRandomMeeting();
            string randomExceptionMessage = Tests.GetRandomString();
            var serviceException = new Exception(randomExceptionMessage);

            var failedMeetingServiceException =
                new FailedMeetingServiceException(serviceException);

            var expectedMeetingServiceException =
                new MeetingServiceException(failedMeetingServiceException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostMeetingAsync(It.IsAny<Meeting>()))
                    .ThrowsAsync(serviceException);

            //Act
            Task<Meeting> addMeetingTask = this.meetingService.AddAsync(someMeeting);

            //Assert
            await Assert.ThrowsAsync<MeetingServiceException>(() => addMeetingTask);

            this.apiBrokerMock.Verify(broker =>
                broker.PostMeetingAsync(It.IsAny<Meeting>()),
                Times.Once);

            Tests.VerifyExceptionLogged(this.loggingBrokerMock, expectedMeetingServiceException);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
