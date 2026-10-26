using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts.Broker.Messages.Booking;
using MapsterMapper;
using MassTransit;

namespace Airbnb.Audit.Infrastructure.Consumers;

public class BookingMessageConsumer : IConsumer<BookingCreated>, IConsumer<BookingUpdated>, IConsumer<BookingDeleted>
{
    private readonly IBookingAuditRepository _bookingAuditRepository;
    private readonly IMapper _mapper;
    
    public BookingMessageConsumer(IBookingAuditRepository bookingAuditRepository, IMapper mapper)
    {
        _bookingAuditRepository = bookingAuditRepository;
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<BookingCreated> context)
    {
        var entity = _mapper.Map<BookingAuditEntity>(context.Message);
        await _bookingAuditRepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<BookingUpdated> context)
    {
        var entity = _mapper.Map<BookingAuditEntity>(context.Message);
        await _bookingAuditRepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<BookingDeleted> context)
    {
        var entity = _mapper.Map<BookingAuditEntity>(context.Message);
        await _bookingAuditRepository.CreateAsync(entity);
    }
}