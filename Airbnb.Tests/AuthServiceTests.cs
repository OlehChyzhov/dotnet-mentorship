using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Authentication;
using Airbnb.Application.Options;
using Airbnb.Application.Services;
using Airbnb.Domain.Constants;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace Airbnb.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserService> _userRepositoryMock;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserService>();

        _jwtOptions = Options.Create(new JwtOptions
        {
            Key = "65C57A40D2BF4EE3851EC843D125770D65C57A40D2BF4EE3851EC843D125770D",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationMinutes = 60
        });

        _mapperMock = new Mock<IMapper>();
        _mapperMock
            .Setup(m => m.Map<UserRegisterRequest, IdentityUser>(It.IsAny<UserRegisterRequest>()))
            .Returns((UserRegisterRequest src) => new IdentityUser { Email = src.Email, UserName = src.Email });

        _sut = new AuthService(_userRepositoryMock.Object, _jwtOptions, _mapperMock.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_WhenRoleDoesNotExist_ReturnsFailedResult()
    {
        // Arrange
        _userRepositoryMock
            .Setup(m => m.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var request = new UserRegisterRequest
        {
            Email = "test@test.com",
            Password = "Password123!",
            Role = Roles.Client
        };

        // Act
        IdentityResult result = await _sut.RegisterUserAsync(request);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "RoleNotFound");

        _userRepositoryMock.Verify(
            m => m.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_WhenRoleExists_CreatesUserAndAddsRole()
    {
        // Arrange
        _userRepositoryMock
            .Setup(m => m.RoleExistsAsync(Roles.Client))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(m => m.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userRepositoryMock
            .Setup(m => m.AddUserToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var request = new UserRegisterRequest
        {
            Email = "test@test.com",
            Password = "Password123!",
            Role = Roles.Client
        };

        // Act
        IdentityResult result = await _sut.RegisterUserAsync(request);

        // Assert
        result.Succeeded.ShouldBeTrue();

        _userRepositoryMock.Verify(
            m => m.CreateUserAsync(
                It.Is<IdentityUser>(u => u.Email == "test@test.com"),
                "Password123!"),
            Times.Once);

        _userRepositoryMock.Verify(
            m => m.AddUserToRoleAsync(It.IsAny<IdentityUser>(), Roles.Client),
            Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_WhenUserNotFound_ReturnsFailure()
    {
        var request = new UserLoginRequest { Email = "missing@test.com", Password = "whatever" };

        // Act
        var result = await _sut.LoginUserAsync(request);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldBe("No user found");
    }

    [Fact]
    public async Task LoginUserAsync_WhenPasswordIsIncorrect_ReturnsFailure()
    {
        // Arrange
        var identityUser = new IdentityUser { Email = "test@test.com" };
        _userRepositoryMock.Setup(m => m.FindUserByEmailAsync("test@test.com")).ReturnsAsync(identityUser);
        _userRepositoryMock.Setup(m => m.CheckPasswordAsync(identityUser, "wrong")).ReturnsAsync(false);

        var request = new UserLoginRequest { Email = "test@test.com", Password = "wrong" };

        // Act
        var result = await _sut.LoginUserAsync(request);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldBe("Incorrect password");
    }

    [Fact]
    public async Task LoginUserAsync_WhenCredentialsAreValid_ReturnsSuccess()
    {
        // Arrange
        var identityUser = new IdentityUser { Email = "test@test.com" };
        _userRepositoryMock.Setup(m => m.FindUserByEmailAsync("test@test.com")).ReturnsAsync(identityUser);
        _userRepositoryMock.Setup(m => m.CheckPasswordAsync(identityUser, "correct")).ReturnsAsync(true);

        var request = new UserLoginRequest { Email = "test@test.com", Password = "correct" };

        // Act
        var result = await _sut.LoginUserAsync(request);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Value.ShouldBe(request);
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
        _userRepositoryMock.Setup(m => m.FindUserByEmailAsync("test@test.com")).ReturnsAsync(identityUser);
        _userRepositoryMock.Setup(m => m.GetRolesAsync(identityUser)).ReturnsAsync(new List<string> { "Client" });

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
        _userRepositoryMock.Setup(m => m.FindUserByEmailAsync("test@test.com")).ReturnsAsync(identityUser);
        _userRepositoryMock.Setup(m => m.GetRolesAsync(identityUser)).ReturnsAsync(new List<string> { "Client", "Host" });

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
