using System;
using Microsoft.Data.SqlClient;
using Moq;
using Parlivote.Shared.Models.Identity.Users.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Users;

public partial class UserServiceTests
{
    [Fact]
    public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogIt()
    {
        // Arrange
        SqlException sqlException = Tests.GetSqlException();

        var failedUserStorageException =
            new FailedUserStorageException(sqlException);

        var expectedUserDependencyException =
            new UserDependencyException(failedUserStorageException);

        this.userManagementBrokerMock.Setup(broker =>
            broker.SelectAllUsers())
                .Throws(sqlException);

        // Act
        Action retrieveUserTask = () =>
            this.userService.RetrieveAll();

        // Assert
        Assert.Throws<UserDependencyException>(retrieveUserTask);

        this.userManagementBrokerMock.Verify(broker =>
            broker.SelectAllUsers(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(Tests.SameExceptionAs(
                expectedUserDependencyException))),
                    Times.Once);

        this.userManagementBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogIt()
    {
        // Arrange
        string randomMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomMessage);

        var failedUserServiceException =
            new FailedUserServiceException(serviceException);

        var expectedUserServiceException =
            new UserServiceException(failedUserServiceException);

        this.userManagementBrokerMock.Setup(broker =>
            broker.SelectAllUsers())
                .Throws(serviceException);

        //Act
        Action retrieveAllUsers = () =>
            this.userService.RetrieveAll();

        //Assert
        Assert.Throws<UserServiceException>(retrieveAllUsers);

        this.userManagementBrokerMock.Verify(broker =>
            broker.SelectAllUsers(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(
                Tests.SameExceptionAs(expectedUserServiceException))),
                    Times.Once);

        this.userManagementBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}
