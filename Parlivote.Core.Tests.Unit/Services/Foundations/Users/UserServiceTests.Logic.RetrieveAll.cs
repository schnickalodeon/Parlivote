using System.Linq;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Identity.Users;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Users;

public partial class UserServiceTests
{
    [Fact]
    public void ShouldRetrieveAllUsers()
    {
        // Arrange
        IQueryable<User> randomUsers = GetRandomUsers();
        IQueryable<User> storageUsers = randomUsers;
        IQueryable<User> expectedUsers = randomUsers.DeepClone();

        this.userManagementBrokerMock.Setup(broker =>
            broker.SelectAllUsers())
            .Returns(storageUsers);

        // Act
        IQueryable<User> actualUsers =
            this.userService.RetrieveAll();

        // Assert
        actualUsers.Should().BeEquivalentTo(expectedUsers);

        this.userManagementBrokerMock.Verify(broker =>
            broker.SelectAllUsers(),
            Times.Once);

        this.userManagementBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}
