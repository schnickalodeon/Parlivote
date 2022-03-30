using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Moq;
using Parlivote.Shared.Models.Meetings;
using Parlivote.Shared.Models.Meetings.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Meetings;

public partial class MeetingServiceTests
{
    [Fact]
    public void ThrowsCriticalDependencyException_OnRetrieveAllIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Meeting someMeeting = GetRandomMeeting();
        Meeting inputMeeting = someMeeting;

        SqlException sqlException = Tests.GetSqlException();

        var meetingStorageException =
            new FailedMeetingStorageException(sqlException);

        var expectedMeetingDependencyException =
            new MeetingDependencyException(meetingStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllMeetings())
                .Throws(sqlException);

        // Act
        Action retrieveAllAction = () => this.meetingService.RetrieveAll();

        // Assert
        Assert.Throws<MeetingDependencyException>(retrieveAllAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllMeetings(),
            Times.Once);

        Tests.VerifyCriticalExceptionLogged(
            this.loggingBrokerMock, 
            expectedMeetingDependencyException);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}