using System.Text.Json;
using Airbnb.Application.Abstracts.Helpers;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.External;
using Airbnb.Application.Helpers;
using Airbnb.Application.Options;
using Airbnb.Domain.Constants;
using Airbnb.Domain.Models;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace Airbnb.Tests;

public class ExternalDataLoaderTests : IDisposable
{
    private readonly Mock<IValidator<ExternalHostDto>> _validatorMock;
    private readonly Mock<IUserRegistrationService> _userHelperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IApartmentRepository> _apartmentRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ExternalDataLoader _sut;

    private readonly List<string> _filesToCleanUp = new();

    public ExternalDataLoaderTests()
    {
        _validatorMock = new Mock<IValidator<ExternalHostDto>>();
        _userHelperMock = new Mock<IUserRegistrationService>();

        _apartmentRepositoryMock = new Mock<IApartmentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Apartments).Returns(_apartmentRepositoryMock.Object);

        _mapperMock = new Mock<IMapper>();

        var defaultUserOptions = new DefaultUserOptions { DefaultPassword = "P@ssword1" };

        _sut = new ExternalDataLoader(
            Options.Create(defaultUserOptions),
            _validatorMock.Object,
            _userHelperMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    public void Dispose()
    {
        foreach (string path in _filesToCleanUp)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static ExternalHostDto CreateHostDto(
        string externalId = "8f14e45f-ceea-4d5c-b0b6-93e13f7f5e2a",
        string email = "host@bookly-legacy.com") => new()
    {
        ExternalId = externalId,
        Email = email,
        Apartments =
        [
            new ExternalApartmentDto
            {
                ExternalId = "3d4b1c2e-7a9f-4a1b-8e3d-1f2a3b4c5d6e",
                Title = "Sunny Studio",
                Description = "Cozy studio",
                Country = "Poland",
                City = "Krakow",
                Address = "Ulica Florianska 12",
                MaxGuests = 2,
                PricePerNight = 65.5,
                IsListed = true,
                CreatedAt = DateTime.UtcNow
            }
        ]
    };
    
    private static ExternalHostDto MatchingHost(ExternalHostDto original) =>
        It.Is<ExternalHostDto>(h => h.ExternalId == original.ExternalId);

    private string WriteHostsFile(params ExternalHostDto[] hosts)
    {
        string fileName = $"{Guid.NewGuid()}.json";
        string filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        File.WriteAllText(filePath, JsonSerializer.Serialize(hosts));
        _filesToCleanUp.Add(filePath);

        return filePath;
    }

    [Fact]
    public async Task LoadDataFromJsonFileAsync_WhenFileNameIsEmpty_ReturnsFailureWithoutStartingTransaction()
    {
        // Act
        var result = await _sut.LoadDataFromJsonFileAsync(string.Empty);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldBe("File name is empty");

        _unitOfWorkMock.Verify(u => u.StartTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task LoadDataFromJsonFileAsync_WhenFileDoesNotExist_ReturnsFailureWithoutStartingTransaction()
    {
        // Act
        var result = await _sut.LoadDataFromJsonFileAsync("does-not-exist.json");

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldBe("File doesn't exist");

        _unitOfWorkMock.Verify(u => u.StartTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task LoadDataFromJsonFileAsync_WhenFileIsValid_CreatesHostAndApartments_CommitsTransaction()
    {
        // Arrange
        ExternalHostDto hostDto = CreateHostDto();
        string filePath = WriteHostsFile(hostDto);

        _validatorMock
            .Setup(v => v.ValidateAsync(MatchingHost(hostDto), default))
            .ReturnsAsync(new ValidationResult());

        var user = new User { Id = "generated-user-id", Email = hostDto.Email };
        var apartment = new Apartment { ExternalId = Guid.Parse(hostDto.Apartments[0].ExternalId) };

        _mapperMock.Setup(m => m.Map<User>(MatchingHost(hostDto))).Returns(user);
        _mapperMock
            .Setup(m => m.Map<Apartment>(It.IsAny<ExternalApartmentDto>()))
            .Returns(apartment);

        _userHelperMock
            .Setup(h => h.CreateUserAsync(user, "P@ssword1"))
            .ReturnsAsync(IdentityResult.Success);
        _userHelperMock
            .Setup(h => h.AddUserToRoleAsync(user, Roles.Host))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.LoadDataFromJsonFileAsync(filePath);

        // Assert
        result.IsSuccessful.ShouldBeTrue();

        apartment.OwnerId.ShouldBe(user.Id);

        _userHelperMock.Verify(h => h.CreateUserAsync(user, "P@ssword1"), Times.Once);
        _userHelperMock.Verify(h => h.AddUserToRoleAsync(user, Roles.Host), Times.Once);
        _apartmentRepositoryMock.Verify(r => r.CreateAsync(apartment), Times.Once);

        _unitOfWorkMock.Verify(u => u.StartTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task LoadDataFromJsonFileAsync_WhenValidationFails_RollsBackAndReturnsFailure()
    {
        // Arrange
        ExternalHostDto hostDto = CreateHostDto();
        string filePath = WriteHostsFile(hostDto);

        var failure = new ValidationFailure("Email", "Email is required");
        _validatorMock
            .Setup(v => v.ValidateAsync(MatchingHost(hostDto), default))
            .ReturnsAsync(new ValidationResult([failure]));

        // Act
        var result = await _sut.LoadDataFromJsonFileAsync(filePath);

        // Assert
        result.IsSuccessful.ShouldBeFalse();

        _userHelperMock.Verify(h => h.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _apartmentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Apartment>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task LoadDataFromJsonFileAsync_WhenHostCreationFails_RollsBackAndReturnsFailure()
    {
        // Arrange
        ExternalHostDto hostDto = CreateHostDto();
        string filePath = WriteHostsFile(hostDto);

        _validatorMock
            .Setup(v => v.ValidateAsync(MatchingHost(hostDto), default))
            .ReturnsAsync(new ValidationResult());

        var user = new User { Id = "generated-user-id", Email = hostDto.Email };
        _mapperMock.Setup(m => m.Map<User>(MatchingHost(hostDto))).Returns(user);

        var identityError = new IdentityError { Description = "Email already taken" };
        _userHelperMock
            .Setup(h => h.CreateUserAsync(user, "P@ssword1"))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _sut.LoadDataFromJsonFileAsync(filePath);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldContain("Email already taken");

        _userHelperMock.Verify(h => h.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _apartmentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Apartment>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task LoadDataFromJsonFileAsync_WhenRoleAssignmentFails_RollsBackAndReturnsFailure()
    {
        // Arrange
        ExternalHostDto hostDto = CreateHostDto();
        string filePath = WriteHostsFile(hostDto);

        _validatorMock
            .Setup(v => v.ValidateAsync(MatchingHost(hostDto), default))
            .ReturnsAsync(new ValidationResult());

        var user = new User { Id = "generated-user-id", Email = hostDto.Email };
        _mapperMock.Setup(m => m.Map<User>(MatchingHost(hostDto))).Returns(user);

        _userHelperMock
            .Setup(h => h.CreateUserAsync(user, "P@ssword1"))
            .ReturnsAsync(IdentityResult.Success);

        var identityError = new IdentityError { Description = "Role does not exist" };
        _userHelperMock
            .Setup(h => h.AddUserToRoleAsync(user, Roles.Host))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _sut.LoadDataFromJsonFileAsync(filePath);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldContain("Role does not exist");

        _apartmentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Apartment>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task LoadDataFromJsonFileAsync_WithMultipleHosts_ProcessesEachHostIndependentlyInSameTransaction()
    {
        // Arrange
        ExternalHostDto firstHost = CreateHostDto("8f14e45f-ceea-4d5c-b0b6-93e13f7f5e2a", "first-host@bookly-legacy.com");
        ExternalHostDto secondHost = CreateHostDto("1b2c3d4e-5f6a-4b7c-8d9e-0f1a2b3c4d5e", "second-host@bookly-legacy.com");
        string filePath = WriteHostsFile(firstHost, secondHost);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ExternalHostDto>(), default))
            .ReturnsAsync(new ValidationResult());

        var firstUser = new User { Id = "user-1", Email = firstHost.Email };
        var secondUser = new User { Id = "user-2", Email = secondHost.Email };

        _mapperMock.Setup(m => m.Map<User>(MatchingHost(firstHost))).Returns(firstUser);
        _mapperMock.Setup(m => m.Map<User>(MatchingHost(secondHost))).Returns(secondUser);
        _mapperMock.Setup(m => m.Map<Apartment>(It.IsAny<ExternalApartmentDto>())).Returns(() => new Apartment());

        _userHelperMock
            .Setup(h => h.CreateUserAsync(It.IsAny<User>(), "P@ssword1"))
            .ReturnsAsync(IdentityResult.Success);
        _userHelperMock
            .Setup(h => h.AddUserToRoleAsync(It.IsAny<User>(), Roles.Host))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.LoadDataFromJsonFileAsync(filePath);

        // Assert
        result.IsSuccessful.ShouldBeTrue();

        _userHelperMock.Verify(h => h.CreateUserAsync(firstUser, "P@ssword1"), Times.Once);
        _userHelperMock.Verify(h => h.CreateUserAsync(secondUser, "P@ssword1"), Times.Once);

        _unitOfWorkMock.Verify(u => u.StartTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task LoadDataFromJsonFileAsync_WhenHostAndApartmentAlreadyExist_UpdatesInsteadOfCreating()
    {
        // Arrange
        ExternalHostDto hostDto = CreateHostDto();
        string filePath = WriteHostsFile(hostDto);

        _validatorMock
            .Setup(v => v.ValidateAsync(MatchingHost(hostDto), default))
            .ReturnsAsync(new ValidationResult());

        var existingHost = new User { Id = "existing-user-id", Email = hostDto.Email };
        _userHelperMock
            .Setup(h => h.FindUserByEmailAsync(hostDto.Email))
            .ReturnsAsync(existingHost);

        var existingApartment = new Apartment
        {
            Id = Guid.NewGuid(),
            ExternalId = Guid.Parse(hostDto.Apartments[0].ExternalId),
            Title = "Old title"
        };
        _apartmentRepositoryMock
            .Setup(r => r.GetByExternalIdAsync(existingApartment.ExternalId))
            .ReturnsAsync(existingApartment);

        // Act
        var result = await _sut.LoadDataFromJsonFileAsync(filePath);

        // Assert
        result.IsSuccessful.ShouldBeTrue();

        _userHelperMock.Verify(h => h.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _apartmentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Apartment>()), Times.Never);

        _mapperMock.Verify(
            m => m.Map(
                It.Is<ExternalApartmentDto>(d => d.ExternalId == hostDto.Apartments[0].ExternalId),
                existingApartment),
            Times.Once);
        _apartmentRepositoryMock.Verify(r => r.UpdateAsync(existingApartment), Times.Once);
        existingApartment.OwnerId.ShouldBe(existingHost.Id);

        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
}
