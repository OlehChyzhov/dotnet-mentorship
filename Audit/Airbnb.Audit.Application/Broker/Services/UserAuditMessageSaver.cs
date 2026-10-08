using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts;
using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Broker.Messages;
using FluentValidation;
using MapsterMapper;

namespace Airbnb.Audit.Application.Broker.Services;

public class UserAuditMessageSaver<TMessage> : IMessageSaver<TMessage>
{
    private readonly IUserAuditRepository _userAuditRepository;
    private readonly IValidator<TMessage> _validator;
    private readonly IMapper _mapper;
    
    public UserAuditMessageSaver(
        IUserAuditRepository userAuditRepository, 
        IValidator<TMessage> validator, 
        IMapper mapper)
    {
        _userAuditRepository = userAuditRepository;
        _validator = validator;
        _mapper = mapper;
    }
    
    public async Task<Result<bool>> HandleAsync(TMessage message)
    {
        var result = await _validator.ValidateAsync(message);
        if (!result.IsValid)
        {
            return "Validation Failed";
        }
        
        var entity = _mapper.Map<UserAuditChangeEntity>(message);
        await _userAuditRepository.CreateAsync(entity);

        return true;
    }
}