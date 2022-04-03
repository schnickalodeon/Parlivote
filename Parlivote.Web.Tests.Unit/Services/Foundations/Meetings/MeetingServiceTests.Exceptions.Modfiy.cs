using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using Xeptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Meetings;

public partial class MeetingServiceTests
{
    [Theory]
    [MemberData(nameof(DependencyValidationException))]
    public async Task ShouldThrowAndLogDependencyValidationException_OnModify_IfDependencyValidationExceptionOccurs(
           Xeption dependencyValidationException)
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();

        var expectedMeetingDependencyValidationException =
            new MeetingDependencyValidationException(
                dependencyValidationException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetMeetingById(It.IsAny<Guid>()))
                .ThrowsAsync(dependencyValidationException);

        // Act
        Task<Meeting> modifyMeetingTask =
            this.meetingService.ModifyAsync(someMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyValidationException>(() => modifyMeetingTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMeetingById(someMeeting.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMeetingDependencyValidationException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(CriticalDependencyException))]
    public async Task ShouldThrowAndLogCriticalDependencyException_OnModify_IfCriticalExceptionOccurs(
        Xeption criticalException)
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();

        var expectedMeetingDependencyException =
            new MeetingDependencyException(
                criticalException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetMeetingById(It.IsAny<Guid>()))
                .ThrowsAsync(criticalException);

        // Act
        Task<Meeting> modifyMeetingTask =
            this.meetingService.ModifyAsync(someMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyException>(() => modifyMeetingTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMeetingById(someMeeting.Id),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock,
            expectedMeetingDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(DependencyException))]
    public async Task ShouldThrowAndLogDependencyException_OnModify_IfDependencyExceptionErrorOccurs(
        Xeption dependencyException)
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();

        var expectedMeetingDependencyException =
            new MeetingDependencyException(
                dependencyException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetMeetingById(It.IsAny<Guid>()))
                .ThrowsAsync(dependencyException);

        // Act
        Task<Meeting> modifyMeetingTask =
            this.meetingService.ModifyAsync(someMeeting);

        // Assert
        await Assert.ThrowsAsync<MeetingDependencyException>(() => modifyMeetingTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMeetingById(someMeeting.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(
            this.loggingBrokerMock,
            expectedMeetingDependencyException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
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
            broker.GetMeetingById(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        //Act
        Task<Meeting> modifyMeetingTask = this.meetingService.ModifyAsync(someMeeting);

        //Assert
        await Assert.ThrowsAsync<MeetingServiceException>(() => modifyMeetingTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetMeetingById(someMeeting.Id),
            Times.Once);

        Tests.VerifyExceptionLogged(this.loggingBrokerMock, 
            expectedMeetingServiceException);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}