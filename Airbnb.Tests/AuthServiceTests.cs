using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Airbnb.Application;
using Airbnb.Application.Services;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;

namespace Airbnb.Tests;

public class AuthServiceTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStoreMock.Object, null, null, null, null);

        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c[Constants.JwtKeyKey]).Returns("65C57A40D2BF4EE3851EC843D125770D65C57A40D2BF4EE3851EC843D125770D");
        _configMock.Setup(c => c[Constants.JwtIssuerKey]).Returns("test-issuer");
        _configMock.Setup(c => c[Constants.JwtAudienceKey]).Returns("test-audience");

        _sut = new AuthService(_userManagerMock.Object, _roleManagerMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_WhenRoleDoesNotExist_ReturnsFailedResult()
    {
        // Arrange
        _roleManagerMock
            .Setup(m => m.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var request = new UserRegisterRequest
        {
            Email = "test@test.com",
            Password = "Password123!",
            Role = Role.Client
        };

        // Act
        IdentityResult result = await _sut.RegisterUserAsync(request);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "RoleNotFound");
        
        _userManagerMock.Verify(
            m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_WhenRoleExists_CreatesUserAndAddsRole()
    {
        // Arrange
        _roleManagerMock
            .Setup(m => m.RoleExistsAsync(Role.Client.ToString()))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var request = new UserRegisterRequest
        {
            Email = "test@test.com",
            Password = "Password123!",
            Role = Role.Client
        };

        // Act
        IdentityResult result = await _sut.RegisterUserAsync(request);

        // Assert
        result.Succeeded.ShouldBeTrue();

        _userManagerMock.Verify(
            m => m.CreateAsync(
                It.Is<IdentityUser>(u => u.Email == "test@test.com"),
                "Password123!"),
            Times.Once);

        _userManagerMock.Verify(
            m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), Role.Client.ToString()),
            Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_WhenUserNotFound_ReturnsFailure()
    {
        var request = new UserLoginRequest { Email = "missing@test.com", Password = "whatever" };

        // Act
        var (isSuccessful, message) = await _sut.LoginUserAsync(request);

        // Assert
        isSuccessful.ShouldBeFalse();
        message.ShouldBe("No user found");
    }

    [Fact]
    public async Task LoginUserAsync_WhenPasswordIsIncorrect_ReturnsFailure()
    {
        // Arrange
        var identityUser = new IdentityUser { Email = "test@test.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("test@test.com")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(identityUser, "wrong")).ReturnsAsync(false);

        var request = new UserLoginRequest { Email = "test@test.com", Password = "wrong" };

        // Act
        var (isSuccessful, message) = await _sut.LoginUserAsync(request);

        // Assert
        isSuccessful.ShouldBeFalse();
        message.ShouldBe("Incorrect password");
    }

    [Fact]
    public async Task LoginUserAsync_WhenCredentialsAreValid_ReturnsSuccess()
    {
        // Arrange
        var identityUser = new IdentityUser { Email = "test@test.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("test@test.com")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(identityUser, "correct")).ReturnsAsync(true);

        var request = new UserLoginRequest { Email = "test@test.com", Password = "correct" };

        // Act
        var (isSuccessful, message) = await _sut.LoginUserAsync(request);

        // Assert
        isSuccessful.ShouldBeTrue();
        message.ShouldBe("Login successful");
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_WhenUserNotFound_ReturnsEmptyString()
    {
        // Arrange
        var request = new UserLoginRequest { Email = "missing@test.com", Password = "whatever" };

        // Act
        string token = await _sut.GenerateJwtTokenAsync(request);

        // Assert
        token.ShouldBe(string.Empty);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_WhenUserExists_ReturnsNonEmptyToken()
    {
        // Arrange
        var identityUser = new IdentityUser { Email = "test@test.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("test@test.com")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(m => m.GetRolesAsync(identityUser)).ReturnsAsync(new List<string> { "Client" });

        var request = new UserLoginRequest { Email = "test@test.com", Password = "correct" };

        // Act
        string token = await _sut.GenerateJwtTokenAsync(request);

        // Assert
        token.ShouldNotBeNullOrEmpty();
        token.Split('.').Length.ShouldBe(3);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_WhenUserExists_TokenContainsExpectedClaims()
    {
        // Arrange
        var identityUser = new IdentityUser { Email = "test@test.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("test@test.com")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(m => m.GetRolesAsync(identityUser)).ReturnsAsync(new List<string> { "Client", "Host" });

        var request = new UserLoginRequest { Email = "test@test.com", Password = "correct" };

        // Act
        string token = await _sut.GenerateJwtTokenAsync(request);

        // Assert
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Issuer.ShouldBe("test-issuer");
        jwt.Audiences.ShouldContain("test-audience");

        jwt.Claims.ShouldContain(c => c.Type == ClaimTypes.Email && c.Value == "test@test.com");
        jwt.Claims.ShouldContain(c => c.Type == ClaimTypes.Role && c.Value == "Client");
        jwt.Claims.ShouldContain(c => c.Type == ClaimTypes.Role && c.Value == "Host");
    }
}
