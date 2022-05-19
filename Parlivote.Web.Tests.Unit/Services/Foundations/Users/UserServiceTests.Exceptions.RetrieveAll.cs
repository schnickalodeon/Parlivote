using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Identity.Users.Exceptions;
using RESTFulSense.Exceptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Users;

public partial class UsersServiceTests
{

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfResponseExceptionOccursAndLogItAsync()
    {
        // Arrange
        string someMessage = Tests.GetRandomString();
        var httpResponseMessage = new HttpResponseMessage();

        var httpResponseException =
            new HttpResponseException(
                httpResponseMessage,
                someMessage);

        var failedUserDependencyException =
            new FailedUserDependencyException(httpResponseException);

        var expectedUserDependencyException =
            new UserDependencyException(failedUserDependencyException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllUsersAsync())
                .ThrowsAsync(httpResponseException);

        // Act
        Task<List<User>> retrieveAllTask =
            this.userService.RetrieveAllAsync();

        // Assert
        await Assert.ThrowsAsync<UserDependencyException>(() =>
            retrieveAllTask);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(
            Tests.SameExceptionAs(expectedUserDependencyException))),
            Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllUsersAsync(),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
    {
        // Arrange
        string someMessage = Tests.GetRandomString();
        var exception = new Exception(someMessage);

        var failedUserServiceException =
            new FailedUserServiceException(exception);

        var expectedUserDependencyException =
            new UserServiceException(failedUserServiceException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllUsersAsync())
                .ThrowsAsync(exception);

        // Act
        Task<List<User>> retrieveAllTask =
            this.userService.RetrieveAllAsync();

        // Assert
        await Assert.ThrowsAsync<UserServiceException>(() =>
            retrieveAllTask);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(
            Tests.SameExceptionAs(expectedUserDependencyException))),
            Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllUsersAsync(),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

}
