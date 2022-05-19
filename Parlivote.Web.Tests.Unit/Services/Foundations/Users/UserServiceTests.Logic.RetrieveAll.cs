using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Parlivote.Shared.Models.Identity.Users;
using Xunit;

namespace Parlivote.Web.Tests.Unit.Services.Foundations.Users;
public partial class UsersServiceTests
{
    [Fact]
    public async Task ShouldRetrieveAllUsersAndLogItAsync()
    {
        // Arrange
        List<User> someUsers = GetRandomUsers();
        var storageUsers = someUsers;
        var expectedUsers = storageUsers.DeepClone();

        this.apiBrokerMock.Setup(broker =>
            broker.GetAllUsersAsync())
                .ReturnsAsync(storageUsers);

        // Act
        List<User> actualUsers =
            await this.userService.RetrieveAllAsync();

        // Assert
        actualUsers.Should().BeEquivalentTo(expectedUsers);

        this.apiBrokerMock.Verify(broker =>
            broker.GetAllUsersAsync(),
                Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}
