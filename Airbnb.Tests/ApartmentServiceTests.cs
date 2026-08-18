using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Application.Services;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;
using MapsterMapper;
using Moq;
using Shouldly;

namespace Airbnb.Tests;

public class ApartmentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IApartmentRepository> _apartmentRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ApartmentService _sut;

    public ApartmentServiceTests()
    {
        _apartmentRepositoryMock = new Mock<IApartmentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Apartments).Returns(_apartmentRepositoryMock.Object);

        _mapperMock = new Mock<IMapper>();

        _sut = new ApartmentService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    private static Apartment CreateApartment(Guid id, bool isListed = true) => new()
    {
        Id = id,
        Title = "Cozy flat",
        Description = "Nice place",
        Type = ApartmentType.Apartment,
        Country = "UA",
        City = "Kyiv",
        Address = "Main St 1",
        MaxGuests = 2,
        Bedrooms = 1,
        Bathrooms = 1,
        Kitchens = 1,
        LivingRooms = 1,
        PricePerNight = 50,
        IsListed = isListed,
        CreatedAt = DateTime.UtcNow,
        OwnerId = "owner-1"
    };

    [Fact]
    public async Task GetApartmentByIdAsync_ReturnsMappedApartment()
    {
        // Arrange
        var apartmentId = Guid.NewGuid();
        var apartment = CreateApartment(apartmentId);
        var apartmentDto = new ApartmentDto { Id = apartmentId, Title = apartment.Title };

        _apartmentRepositoryMock.Setup(r => r.GetByIdAsync(apartmentId)).ReturnsAsync(apartment);
        _mapperMock.Setup(m => m.Map<ApartmentDto>(apartment)).Returns(apartmentDto);

        // Act
        var result = await _sut.GetApartmentByIdAsync(apartmentId);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Value.ShouldBe(apartmentDto);
    }

    [Fact]
    public async Task GetApartmentsAsync_ReturnsPagedResultWithMetadata()
    {
        // Arrange
        var query = new ApartmentPagingParamters { PageNumber = 1, PageSize = 10 };

        var apartments = new List<Apartment> { CreateApartment(Guid.NewGuid()), CreateApartment(Guid.NewGuid()) };
        var pagedApartments = PagedList<Apartment>.ToPagedList(apartments, totalCount: 2, pageNumber: 1, pageSize: 10);

        var apartmentDtos = apartments
            .Select(a => new ApartmentDto { Id = a.Id, Title = a.Title })
            .ToList();

        _apartmentRepositoryMock.Setup(r => r.GetApartmentsPagedAsync(query)).ReturnsAsync(pagedApartments);
        _mapperMock.Setup(m => m.Map<List<ApartmentDto>>(pagedApartments)).Returns(apartmentDtos);

        // Act
        var result = await _sut.GetApartmentsAsync(query);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Value!.ShouldBe(apartmentDtos);
        result.Value!.MetaData.TotalCount.ShouldBe(2);
        result.Value!.MetaData.CurrentPage.ShouldBe(1);
        result.Value!.MetaData.PageSize.ShouldBe(10);
    }

    [Fact]
    public async Task CreateApartmentAsync_SetsOwnerIdAndCreatedAt_ReturnsCreatedApartment()
    {
        // Arrange
        var dto = new CreateApartmentDto { Title = "New place", PricePerNight = 100 };
        const string userId = "owner-1";

        var mappedApartment = new Apartment { Title = dto.Title, PricePerNight = dto.PricePerNight };
        _mapperMock.Setup(m => m.Map<Apartment>(dto)).Returns(mappedApartment);

        Apartment? createdApartment = null;
        _apartmentRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Apartment>()))
            .Callback<Apartment>(a => createdApartment = a)
            .Returns(Task.CompletedTask);

        _apartmentRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => createdApartment!);

        var expectedDto = new ApartmentDto { Title = dto.Title };
        _mapperMock.Setup(m => m.Map<ApartmentDto>(It.IsAny<Apartment>())).Returns(expectedDto);

        // Act
        var result = await _sut.CreateApartmentAsync(dto, userId);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Value.ShouldBe(expectedDto);

        createdApartment.ShouldNotBeNull();
        createdApartment!.OwnerId.ShouldBe(userId);
        createdApartment.Id.ShouldNotBe(Guid.Empty);

        _apartmentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Apartment>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
