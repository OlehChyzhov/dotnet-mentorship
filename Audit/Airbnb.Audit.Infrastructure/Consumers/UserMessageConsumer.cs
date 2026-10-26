using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.Messages.User;
using MapsterMapper;
using MassTransit;

namespace Airbnb.Audit.Infrastructure.Consumers;

public class UserMessageConsumer : IConsumer<UserCreated>, IConsumer<UserUpdated>, IConsumer<UserDeleted>
{
    private readonly IUserAuditRepository _userAuditRepository;
    private readonly IMapper _mapper;
    
    public UserMessageConsumer(IUserAuditRepository userAuditRepository, IMapper mapper)
    {
        _userAuditRepository = userAuditRepository;
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var entity = _mapper.Map<UserAuditEntity>(context.Message);
        await _userAuditRepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var entity = _mapper.Map<UserAuditEntity>(context.Message);
        await _userAuditRepository.CreateAsync(entity);
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var entity = _mapper.Map<UserAuditEntity>(context.Message);
        await _userAuditRepository.CreateAsync(entity);
    }
}