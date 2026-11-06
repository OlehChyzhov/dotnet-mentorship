using Airbnb.Application.Abstractions.Repositories;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.Services;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;
using Airbnb.Shared.Contracts.Events.Payment;
using MassTransit;
using Airbnb.Shared.Common;
using Airbnb.Shared.Contracts.Messages;
using MapsterMapper;
using Moq;
using Shouldly;

namespace Airbnb.Tests;

public class BookingServiceTests
{
    private readonly Mock<IRequestClient<CreatePaymentSessionMessage>> _paymentClientMock;
    private readonly Mock<IPublishEndpoint> _publisherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IApartmentRepository> _apartmentRepositoryMock;
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BookingService _sut;

    public BookingServiceTests()
    {
        _paymentClientMock = new Mock<IRequestClient<CreatePaymentSessionMessage>>();
        _publisherMock = new Mock<IPublishEndpoint>();
        _apartmentRepositoryMock = new Mock<IApartmentRepository>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Apartments).Returns(_apartmentRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Bookings).Returns(_bookingRepositoryMock.Object);

        _mapperMock = new Mock<IMapper>();

        _sut = new BookingService(_paymentClientMock.Object, _publisherMock.Object, _unitOfWorkMock.Object, _mapperMock.Object);
    }

    private static Apartment CreateApartment(Guid id, bool isListed = true, decimal pricePerNight = 50) => new()
    {
        Id = id,
        Title = "Cozy flat",
        IsListed = isListed,
        PricePerNight = pricePerNight,
        OwnerId = "owner-1"
    };

    private void SetupPaymentSessionResponse(object message)
    {
        var succeeded = new TaskCompletionSource<Response<PaymentSessionCreationSucceededEvent>>();
        var failed = new TaskCompletionSource<Response<PaymentSessionCreationFailedEvent>>();

        switch (message)
        {
            case PaymentSessionCreationSucceededEvent s:
                var ok = new Mock<Response<PaymentSessionCreationSucceededEvent>>();
                ok.Setup(r => r.Message).Returns(s);
                succeeded.SetResult(ok.Object);
                failed.SetCanceled();
                break;
            case PaymentSessionCreationFailedEvent f:
                var fail = new Mock<Response<PaymentSessionCreationFailedEvent>>();
                fail.Setup(r => r.Message).Returns(f);
                failed.SetResult(fail.Object);
                succeeded.SetCanceled();
                break;
        }

        _paymentClientMock
            .Setup(c => c.GetResponse<PaymentSessionCreationSucceededEvent, PaymentSessionCreationFailedEvent>(
                It.IsAny<CreatePaymentSessionMessage>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ReturnsAsync(new Response<PaymentSessionCreationSucceededEvent, PaymentSessionCreationFailedEvent>(
                succeeded.Task, failed.Task));
    }

    private static Booking CreateBooking(Guid id, Guid apartmentId) => new()
    {
        Id = id,
        ApartmentId = apartmentId,
        Status = BookingStatus.AwaitingPayment,
        CheckIn = DateTime.UtcNow.AddDays(1),
        CheckOut = DateTime.UtcNow.AddDays(3),
        GuestsCount = 2,
        ClientId = "client-1"
    };

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsMappedBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = CreateBooking(bookingId, Guid.NewGuid());
        var bookingDto = new BookingDto { Id = bookingId };

        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _mapperMock.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        // Act
        var result = await _sut.GetBookingByIdAsync(bookingId);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Value.ShouldBe(bookingDto);
    }

    [Fact]
    public async Task GetBookingsAsync_ReturnsPagedResultWithMetadata()
    {
        // Arrange
        const string userId = "client-1";
        var query = new BookingPagingPrams { PageNumber = 1, PageSize = 10 };

        var bookings = new List<Booking> { CreateBooking(Guid.NewGuid(), Guid.NewGuid()) };
        var pagedBookings = new PagedResult<Booking>(bookings, totalCount: 1, pageNumber: 1, pageSize: 10);

        var bookingDtos = bookings.Select(b => new BookingDto { Id = b.Id }).ToList();

        _bookingRepositoryMock.Setup(r => r.GetBookingsPagedAsync(query, userId)).ReturnsAsync(pagedBookings);
        _mapperMock.Setup(m => m.Map<List<BookingDto>>(pagedBookings.Items)).Returns(bookingDtos);

        // Act
        var result = await _sut.GetBookingsAsync(query, userId);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Value!.Items.ShouldBe(bookingDtos);
        result.Value!.Metadata.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenApartmentAlreadyBookedInRange_ReturnsFailure()
    {
        // Arrange
        var apartmentId = Guid.NewGuid();
        var dto = new CreateBookingDto
        {
            ApartmentId = apartmentId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(3),
            GuestsCount = 2
        };

        _apartmentRepositoryMock.Setup(r => r.GetByIdAsync(apartmentId)).ReturnsAsync(CreateApartment(apartmentId));
        _bookingRepositoryMock
            .Setup(r => r.GetConfirmedOrPendingBookingsInTimeRangeAsync(apartmentId, dto.CheckIn, dto.CheckOut))
            .ReturnsAsync(new List<Booking> { CreateBooking(Guid.NewGuid(), apartmentId) });

        // Act
        var result = await _sut.CreateBookingAsync(dto, "client-1");

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldBe("The apartment is already booked for the selected time range");

        _bookingRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenApartmentIsNotListed_ReturnsFailure()
    {
        // Arrange
        var apartmentId = Guid.NewGuid();
        var dto = new CreateBookingDto
        {
            ApartmentId = apartmentId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(3),
            GuestsCount = 2
        };

        _apartmentRepositoryMock.Setup(r => r.GetByIdAsync(apartmentId)).ReturnsAsync(CreateApartment(apartmentId, isListed: false));
        _bookingRepositoryMock
            .Setup(r => r.GetConfirmedOrPendingBookingsInTimeRangeAsync(apartmentId, dto.CheckIn, dto.CheckOut))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _sut.CreateBookingAsync(dto, "client-1");

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldBe("Can't create booking for an unlisted apartment");

        _bookingRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenValid_CalculatesPriceAndCreatesBooking()
    {
        // Arrange
        var apartmentId = Guid.NewGuid();
        const string userId = "client-1";
        const decimal pricePerNight = 50;

        var baseDate = DateTime.UtcNow;
        var checkIn = baseDate.AddDays(1);
        var checkOut = baseDate.AddDays(4); // 3 nights

        var dto = new CreateBookingDto
        {
            ApartmentId = apartmentId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            GuestsCount = 2
        };

        _apartmentRepositoryMock
            .Setup(r => r.GetByIdAsync(apartmentId))
            .ReturnsAsync(CreateApartment(apartmentId, isListed: true, pricePerNight: pricePerNight));

        _bookingRepositoryMock
            .Setup(r => r.GetConfirmedOrPendingBookingsInTimeRangeAsync(apartmentId, checkIn, checkOut))
            .ReturnsAsync(new List<Booking>());

        _mapperMock.Setup(m => m.Map<Booking>(dto)).Returns(new Booking { ApartmentId = apartmentId, CheckIn = checkIn, CheckOut = checkOut });

        Booking? createdBooking = null;
        _bookingRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Booking>()))
            .Callback<Booking>(b => createdBooking = b)
            .Returns(Task.CompletedTask);

        _bookingRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => createdBooking!);

        var expectedDto = new BookingDto { Id = Guid.NewGuid() };
        _mapperMock.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>())).Returns(expectedDto);

        SetupPaymentSessionResponse(new PaymentSessionCreationSucceededEvent { PaymentSessionUrl = "https://pay.test/session" });

        // Act
        var result = await _sut.CreateBookingAsync(dto, userId);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Value.ShouldBe(expectedDto);

        createdBooking.ShouldNotBeNull();
        createdBooking!.ClientId.ShouldBe(userId);
        createdBooking.Status.ShouldBe(BookingStatus.AwaitingPayment);
        createdBooking.BookedPricePerNight.ShouldBe(pricePerNight);
        createdBooking.BookedTotalPrice.ShouldBe(pricePerNight * 3);
        createdBooking.PaymentSessionUrl.ShouldBe("https://pay.test/session");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenPaymentSessionCreationFails_ReturnsFailureAndDoesNotCreateBooking()
    {
        // Arrange
        var apartmentId = Guid.NewGuid();
        var dto = new CreateBookingDto
        {
            ApartmentId = apartmentId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(3),
            GuestsCount = 2
        };

        _apartmentRepositoryMock.Setup(r => r.GetByIdAsync(apartmentId)).ReturnsAsync(CreateApartment(apartmentId));
        _bookingRepositoryMock
            .Setup(r => r.GetConfirmedOrPendingBookingsInTimeRangeAsync(apartmentId, dto.CheckIn, dto.CheckOut))
            .ReturnsAsync(new List<Booking>());
        _mapperMock.Setup(m => m.Map<Booking>(dto)).Returns(new Booking { ApartmentId = apartmentId });

        SetupPaymentSessionResponse(new PaymentSessionCreationFailedEvent { FailureReason = "Card provider down" });

        // Act
        var result = await _sut.CreateBookingAsync(dto, "client-1");

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldBe("Card provider down");

        _bookingRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Booking>()), Times.Never);
        _publisherMock.Verify(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenPaymentServiceTimesOut_ReturnsFailureAndDoesNotCreateBooking()
    {
        // Arrange
        var apartmentId = Guid.NewGuid();
        var dto = new CreateBookingDto
        {
            ApartmentId = apartmentId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(3),
            GuestsCount = 2
        };

        _apartmentRepositoryMock.Setup(r => r.GetByIdAsync(apartmentId)).ReturnsAsync(CreateApartment(apartmentId));
        _bookingRepositoryMock
            .Setup(r => r.GetConfirmedOrPendingBookingsInTimeRangeAsync(apartmentId, dto.CheckIn, dto.CheckOut))
            .ReturnsAsync(new List<Booking>());
        _mapperMock.Setup(m => m.Map<Booking>(dto)).Returns(new Booking { ApartmentId = apartmentId });

        _paymentClientMock
            .Setup(c => c.GetResponse<PaymentSessionCreationSucceededEvent, PaymentSessionCreationFailedEvent>(
                It.IsAny<CreatePaymentSessionMessage>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ThrowsAsync(new RequestTimeoutException());

        // Act
        var result = await _sut.CreateBookingAsync(dto, "client-1");

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Message.ShouldBe("Could not create payment session");

        _bookingRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Booking>()), Times.Never);
    }
}
