using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Moq;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Identity.Users.Exceptions;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Users;

public partial class UserServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
    {
        // Arrange
        Guid someGuid = Guid.NewGuid();
        SqlException sqlException = Tests.GetSqlException();

        var failedUserStorageException =
            new FailedUserStorageException(sqlException);

        var expectedUserDependencyException =
            new UserDependencyException(failedUserStorageException);

        this.userManagementBrokerMock.Setup(broker =>
            broker.SelectUserByIdAsync(someGuid))
            .Throws(sqlException);

        // Act
        Task<User> retrieveUserByIdTask =
            this.userService.RetrieveByIdAsync(someGuid);

        // Assert
        await Assert.ThrowsAsync<UserDependencyException>(() =>
            retrieveUserByIdTask);

        this.userManagementBrokerMock.Verify(broker =>
            broker.SelectUserByIdAsync(someGuid),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(Tests.SameExceptionAs(
                expectedUserDependencyException))),
                    Times.Once);

        this.userManagementBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
    {
        // Arrange
        Guid someGuid = Guid.NewGuid();
        string randomMessage = Tests.GetRandomString();
        var serviceException = new Exception(randomMessage);

        var failedUserServiceException =
            new FailedUserServiceException(serviceException);

        var expectedUserServiceException =
            new UserServiceException(failedUserServiceException);

        this.userManagementBrokerMock.Setup(broker =>
            broker.SelectUserByIdAsync(someGuid))
                .ThrowsAsync(serviceException);

        //Act
        Task<User> retrieveByIdUsersTask =
            this.userService.RetrieveByIdAsync(someGuid);

        //Assert
        await Assert.ThrowsAsync<UserServiceException>(() =>
            retrieveByIdUsersTask);

        this.userManagementBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(someGuid),
            Times.Once);

        this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    Tests.SameExceptionAs(expectedUserServiceException))),
                        Times.Once);

        this.userManagementBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}
