using Airbnb.Application.Abstracts.Identity;
using Airbnb.Application.Services;
using Airbnb.Domain.Constants;
using Airbnb.Domain.Models;
using Airbnb.Messaging.Broker.Publisher;
using Airbnb.Messaging.Messages;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;

namespace Airbnb.Tests;

public class UserServiceTests
{
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _identityServiceMock = new Mock<IIdentityService>();
        _eventPublisherMock = new Mock<IEventPublisher>();

        _sut = new UserService(_identityServiceMock.Object, _eventPublisherMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_WhenRoleDoesNotExist_ReturnsFailedResultWithoutCreatingUser()
    {
        // Arrange
        _identityServiceMock
            .Setup(m => m.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var user = new User { Email = "test@test.com" };

        // Act
        IdentityResult result = await _sut.CreateUserAsync(user, "Password123!", Roles.Client);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "RoleNotFound");

        _identityServiceMock.Verify(
            m => m.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _eventPublisherMock.Verify(
            m => m.PublishAsync(It.IsAny<UserCreated>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WhenRoleExists_CreatesUserAddsRoleAndPublishesEvent()
    {
        // Arrange
        _identityServiceMock.Setup(m => m.RoleExistsAsync(Roles.Client)).ReturnsAsync(true);
        _identityServiceMock
            .Setup(m => m.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _identityServiceMock
            .Setup(m => m.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var user = new User { Id = "user-id", Email = "test@test.com", UserName = "test@test.com" };

        // Act
        IdentityResult result = await _sut.CreateUserAsync(user, "Password123!", Roles.Client);

        // Assert
        result.Succeeded.ShouldBeTrue();

        _identityServiceMock.Verify(m => m.CreateUserAsync(user, "Password123!"), Times.Once);
        _identityServiceMock.Verify(m => m.AddUserToRoleAsync(user, Roles.Client), Times.Once);
        _eventPublisherMock.Verify(
            m => m.PublishAsync(It.Is<UserCreated>(msg => msg.UserId == "user-id" && msg.Email == "test@test.com")),
            Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WhenRoleAssignmentFails_DoesNotPublishEvent()
    {
        // Arrange
        _identityServiceMock.Setup(m => m.RoleExistsAsync(Roles.Host)).ReturnsAsync(true);
        _identityServiceMock
            .Setup(m => m.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _identityServiceMock
            .Setup(m => m.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" }));

        var user = new User { Id = "user-id", Email = "test@test.com" };

        // Act
        IdentityResult result = await _sut.CreateUserAsync(user, "Password123!", Roles.Host);

        // Assert
        result.Succeeded.ShouldBeFalse();
        _eventPublisherMock.Verify(
            m => m.PublishAsync(It.IsAny<UserCreated>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEmailAsync_WhenUserNotFound_ReturnsUserNotFound()
    {
        // Act
        IdentityResult result = await _sut.UpdateEmailAsync("missing-id", "new@test.com");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "UserNotFound");

        _identityServiceMock.Verify(
            m => m.ChangeEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEmailAsync_WhenChangeSucceeds_PublishesEventWithOldAndNewEmail()
    {
        // Arrange
        var user = new User { Id = "user-id", Email = "old@test.com" };
        _identityServiceMock.Setup(m => m.FindUserByIdAsync("user-id")).ReturnsAsync(user);
        _identityServiceMock
            .Setup(m => m.ChangeEmailAsync(user, "new@test.com"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        IdentityResult result = await _sut.UpdateEmailAsync("user-id", "new@test.com");

        // Assert
        result.Succeeded.ShouldBeTrue();
        _eventPublisherMock.Verify(
            m => m.PublishAsync(It.Is<UserEmailChanged>(msg =>
                msg.UserId == "user-id" && msg.OldEmail == "old@test.com" && msg.NewEmail == "new@test.com")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateEmailAsync_WhenChangeFails_DoesNotPublishEvent()
    {
        // Arrange
        var user = new User { Id = "user-id", Email = "old@test.com" };
        _identityServiceMock.Setup(m => m.FindUserByIdAsync("user-id")).ReturnsAsync(user);
        _identityServiceMock
            .Setup(m => m.ChangeEmailAsync(user, "taken@test.com"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email already taken" }));

        // Act
        IdentityResult result = await _sut.UpdateEmailAsync("user-id", "taken@test.com");

        // Assert
        result.Succeeded.ShouldBeFalse();
        _eventPublisherMock.Verify(
            m => m.PublishAsync(It.IsAny<UserEmailChanged>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenDeleteSucceeds_PublishesEvent()
    {
        // Arrange
        var user = new User { Id = "user-id", Email = "test@test.com" };
        _identityServiceMock.Setup(m => m.FindUserByIdAsync("user-id")).ReturnsAsync(user);
        _identityServiceMock.Setup(m => m.DeleteUserAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        IdentityResult result = await _sut.DeleteUserAsync("user-id");

        // Assert
        result.Succeeded.ShouldBeTrue();
        _eventPublisherMock.Verify(
            m => m.PublishAsync(It.Is<UserDeleted>(msg => msg.UserId == "user-id" && msg.Email == "test@test.com")),
            Times.Once);
    }
}
