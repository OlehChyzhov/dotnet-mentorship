using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.Contracts.Events.User;
using MapsterMapper;
using MassTransit;

namespace Airbnb.Audit.Infrastructure.Consumers;

public class UserMessageConsumer : IConsumer<UserCreatedEvent>, IConsumer<UserUpdatedEvent>, IConsumer<UserDeletedEvent>
{
    private readonly IUserAuditRepository _userAuditRepository;
    private readonly IMapper _mapper;
    
    public UserMessageConsumer(IUserAuditRepository userAuditRepository, IMapper mapper)
    {
        _userAuditRepository = userAuditRepository;
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var entity = _mapper.Map<UserAuditEntity>(context.Message);
        await _userAuditRepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
    {
        var entity = _mapper.Map<UserAuditEntity>(context.Message);
        await _userAuditRepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        var entity = _mapper.Map<UserAuditEntity>(context.Message);
        await _userAuditRepository.CreateAsync(entity);
    }
}