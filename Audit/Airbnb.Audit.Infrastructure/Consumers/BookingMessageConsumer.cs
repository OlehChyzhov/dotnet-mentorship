using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.Contracts.Events.Booking;
using MapsterMapper;
using MassTransit;

namespace Airbnb.Audit.Infrastructure.Consumers;

public class BookingMessageConsumer : IConsumer<BookingCreatedEvent>, IConsumer<BookingUpdatedEvent>, IConsumer<BookingDeletedEvent>
{
    private readonly IBookingAuditRepository _bookingAuditRepository;
    private readonly IMapper _mapper;
    
    public BookingMessageConsumer(IBookingAuditRepository bookingAuditRepository, IMapper mapper)
    {
        _bookingAuditRepository = bookingAuditRepository;
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var entity = _mapper.Map<BookingAuditEntity>(context.Message);
        await _bookingAuditRepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<BookingUpdatedEvent> context)
    {
        var entity = _mapper.Map<BookingAuditEntity>(context.Message);
        await _bookingAuditRepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<BookingDeletedEvent> context)
    {
        var entity = _mapper.Map<BookingAuditEntity>(context.Message);
        await _bookingAuditRepository.CreateAsync(entity);
    }
}