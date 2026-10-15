using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts.Broker.Messages.User;
using MapsterMapper;
using MassTransit;

namespace Airbnb.Audit.Application.Broker.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IUserAuditRepository _userAuditRepository;
    private readonly IMapper _mapper;
    
    public UserCreatedConsumer(IUserAuditRepository userAuditRepository, IMapper mapper)
    {
        _userAuditRepository = userAuditRepository;
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var entity = _mapper.Map<UserAuditChangeEntity>(context.Message);
        await _userAuditRepository.CreateAsync(entity);
    }
}