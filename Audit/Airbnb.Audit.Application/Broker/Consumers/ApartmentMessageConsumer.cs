using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts.Broker.Messages.Apartment;
using MapsterMapper;
using MassTransit;

namespace Airbnb.Audit.Application.Broker.Consumers;

public class ApartmentMessageConsumer : IConsumer<ApartmentCreated>, IConsumer<ApartmentUpdated>, IConsumer<ApartmentDeleted>
{
    private readonly IApartmentAuditRepository _apartmentAuditrepository;
    private readonly IMapper _mapper;
    
    public ApartmentMessageConsumer(IApartmentAuditRepository apartmentAuditrepository, IMapper mapper)
    {
        _apartmentAuditrepository = apartmentAuditrepository;
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<ApartmentCreated> context)
    {
        var entity = _mapper.Map<ApartmentAuditEntity>(context.Message);
        await _apartmentAuditrepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<ApartmentUpdated> context)
    {
        var entity = _mapper.Map<ApartmentAuditEntity>(context.Message);
        await _apartmentAuditrepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<ApartmentDeleted> context)
    {
        var entity = _mapper.Map<ApartmentAuditEntity>(context.Message);
        await _apartmentAuditrepository.CreateAsync(entity);
    }
}