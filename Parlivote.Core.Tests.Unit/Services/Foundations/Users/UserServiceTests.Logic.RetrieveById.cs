using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Parlivote.Shared.Models.Identity.Users;
using Xunit;

namespace Parlivote.Core.Tests.Unit.Services.Foundations.Users;

public partial class UserServiceTests
{
    [Fact]
    public async Task ShouldRetrieveUserByIdAsync()
    {
        // Arrange
        User someUser = GetRandomUser();
        User storageUser = someUser;
        User expectedUser = storageUser;

        this.userManagementBrokerMock.Setup(broker =>
            broker.SelectUserByIdAsync(someUser.Id))
            .ReturnsAsync(storageUser);

        // Act
        User actualUser =
            await this.userService.RetrieveByIdAsync(someUser.Id);

        // Assert
        actualUser.Should().BeEquivalentTo(expectedUser);

        this.userManagementBrokerMock.Verify(broker =>
            broker.SelectUserByIdAsync(someUser.Id),
            Times.Once);

        this.userManagementBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}