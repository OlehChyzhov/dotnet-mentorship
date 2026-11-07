using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.Contracts.Events.Apartment;
using MapsterMapper;
using MassTransit;

namespace Airbnb.Audit.Infrastructure.Consumers;

public class ApartmentMessageConsumer : IConsumer<ApartmentCreatedEvent>, IConsumer<ApartmentUpdatedEvent>, IConsumer<ApartmentDeletedEvent>
{
    private readonly IApartmentAuditRepository _apartmentAuditrepository;
    private readonly IMapper _mapper;
    
    public ApartmentMessageConsumer(IApartmentAuditRepository apartmentAuditrepository, IMapper mapper)
    {
        _apartmentAuditrepository = apartmentAuditrepository;
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<ApartmentCreatedEvent> context)
    {
        var entity = _mapper.Map<ApartmentAuditEntity>(context.Message);
        await _apartmentAuditrepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<ApartmentUpdatedEvent> context)
    {
        var entity = _mapper.Map<ApartmentAuditEntity>(context.Message);
        await _apartmentAuditrepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<ApartmentDeletedEvent> context)
    {
        var entity = _mapper.Map<ApartmentAuditEntity>(context.Message);
        await _apartmentAuditrepository.CreateAsync(entity);
    }
}