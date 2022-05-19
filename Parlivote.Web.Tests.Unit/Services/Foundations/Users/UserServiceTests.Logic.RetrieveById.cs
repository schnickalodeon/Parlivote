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
    public async Task ShouldRetrieveByIdAsync()
    {
        User someUser = GetRandomUser();
        User storageUser = someUser;
        User expectedUser = storageUser.DeepClone();

        this.apiBrokerMock.Setup(broker =>
            broker.GetUserByIdAsync(someUser.Id))
                .ReturnsAsync(storageUser);

        // Act
        User actualUser =
            await this.userService.RetrieveByIdAsync(someUser.Id);

        // Assert
        actualUser.Should().BeEquivalentTo(expectedUser);

        this.apiBrokerMock.Verify(broker =>
            broker.GetUserByIdAsync(expectedUser.Id),
                Times.Once);

        this.apiBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}