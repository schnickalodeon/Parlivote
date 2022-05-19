using System;
using System.Threading.Tasks;
using Moq;
using Parlivote.Shared.Models;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Shared.Models.Identity.Users.Exceptions;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Users;

public partial class UsersServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnRetrieveUserByIdIfUserIdIsInvalidAndLogItAsync()
    {
        Guid userId = Guid.Empty;

        var invalidUserException = new InvalidUserException();

        invalidUserException.AddData(
            key: nameof(User.Id),
            values: ExceptionMessages.INVALID_ID);

        var expectedUserValidationException =
            new UserValidationException(invalidUserException);

        // Act
        Task<User> retrieveUserByIdTask =
            this.userService.RetrieveByIdAsync(userId);

        // Assert
        await Assert.ThrowsAsync<UserValidationException>(() =>
            retrieveUserByIdTask);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(
                Tests.SameExceptionAs(expectedUserValidationException))),
                    Times.Once);

        this.apiBrokerMock.Verify(broker =>
            broker.GetUserByIdAsync(userId),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.apiBrokerMock.VerifyNoOtherCalls();
    }
}