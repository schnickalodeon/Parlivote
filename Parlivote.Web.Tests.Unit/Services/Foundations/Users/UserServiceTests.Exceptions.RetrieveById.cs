using System;
using System.Collections;
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
    [Theory]
    [MemberData(nameof(CriticalDependencyExceptions))]
    public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfCriticalErrorOccursAndLogItAsync(
        Exception criticalDependencyException)
    {
        // Arrange
        Guid someUserId = Guid.NewGuid();

        var failedUserDependencyException =
            new FailedUserDependencyException(criticalDependencyException);

        var expectedUserDependencyException =
            new UserDependencyException(failedUserDependencyException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetUserByIdAsync(someUserId))
            .ThrowsAsync(criticalDependencyException);

        // Act
        Task<User> retrieveByIdTask =
            this.userService.RetrieveByIdAsync(someUserId);

        // Assert
        await Assert.ThrowsAsync<UserDependencyException>(() =>
            retrieveByIdTask);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(
            Tests.SameExceptionAs(expectedUserDependencyException))),
            Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.GetUserByIdAsync(It.IsAny<Guid>()),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfNotFoundExceptionOccursAndLogItAsync()
    {
        // Arrange
        Guid someUserId = Guid.NewGuid();
        IDictionary randomDictionary = CreateRandomDictionary();
        IDictionary exceptionData = randomDictionary;
        string someMessage = Tests.GetRandomString();
        var someResponseMessage = new HttpResponseMessage();

        var httpResponseNotFoundException =
            new HttpResponseNotFoundException(
                someResponseMessage,
                someMessage);

        httpResponseNotFoundException.AddData(exceptionData);

        var notFoundUserException =
            new NotFoundUserException(
                httpResponseNotFoundException);

        var expectedUserDependencyValidationException =
            new UserDependencyValidationException(notFoundUserException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetUserByIdAsync(someUserId))
            .ThrowsAsync(httpResponseNotFoundException);

        // Act
        Task<User> retrieveUserByIdTask =
            this.userService.RetrieveByIdAsync(someUserId);

        // Assert
        await Assert.ThrowsAsync<UserDependencyValidationException>(() =>
            retrieveUserByIdTask);

        this.apiBrokerMock.Verify(broker =>
            broker.GetUserByIdAsync(someUserId),
            Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(
            Tests.SameExceptionAs(expectedUserDependencyValidationException))),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyValidationExceptionsOnRetrieveByIdIfBadRequestExceptionOccursAndLogItAsync()
    {
        // Arrange
        Guid someUserId = Guid.NewGuid();
        IDictionary randomDictionary = CreateRandomDictionary();
        IDictionary exceptionData = randomDictionary;
        string someMessage = Tests.GetRandomString();
        var someResponseMessage = new HttpResponseMessage();

        var httpResponseBadRequestException =
            new HttpResponseBadRequestException(
                someResponseMessage,
                someMessage);

        httpResponseBadRequestException.AddData(exceptionData);

        var invalidUserException =
            new InvalidUserException(
                httpResponseBadRequestException,
                httpResponseBadRequestException.Data);

        var expectedUserDependencyValidationException =
            new UserDependencyValidationException(invalidUserException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetUserByIdAsync(someUserId))
                .ThrowsAsync(httpResponseBadRequestException);

        // Act
        Task<User> retrieveUserByIdAsync =
            this.userService.RetrieveByIdAsync(someUserId);

        // Assert
        await Assert.ThrowsAsync<UserDependencyValidationException>(() =>
            retrieveUserByIdAsync);

        this.apiBrokerMock.Verify(broker =>
            broker.GetUserByIdAsync(someUserId),
            Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(
            Tests.SameExceptionAs(expectedUserDependencyValidationException))),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfResponseExceptionOccursAndLogItAsync()
    {
        // Arrange
        Guid someUserId = Guid.NewGuid();
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
            broker.GetUserByIdAsync(someUserId))
                .ThrowsAsync(httpResponseException);

        // Act
        Task<User> retrieveUserByIdAsync =
            this.userService.RetrieveByIdAsync(someUserId);

        // Assert
        await Assert.ThrowsAsync<UserDependencyException>(() =>
            retrieveUserByIdAsync);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(
            Tests.SameExceptionAs(expectedUserDependencyException))),
            Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.GetUserByIdAsync(It.IsAny<Guid>()),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
    {
        // Arrange
        Guid someUserId = Guid.NewGuid();
        string someMessage = Tests.GetRandomString();
        var exception = new Exception(someMessage);

        var failedUserServiceException =
            new FailedUserServiceException(exception);

        var expectedUserDependencyException =
            new UserServiceException(failedUserServiceException);

        this.apiBrokerMock.Setup(broker =>
            broker.GetUserByIdAsync(someUserId))
                .ThrowsAsync(exception);

        // Act
        Task<User> retrieveUserByIdAsync =
            this.userService.RetrieveByIdAsync(someUserId);

        // Assert
        await Assert.ThrowsAsync<UserServiceException>(() =>
            retrieveUserByIdAsync);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(
            Tests.SameExceptionAs(expectedUserDependencyException))),
            Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.GetUserByIdAsync(It.IsAny<Guid>()),
            Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

}