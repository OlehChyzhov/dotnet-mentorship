using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts;
using Airbnb.Contracts.Broker;
using FluentValidation;
using MapsterMapper;

namespace Airbnb.Audit.Application.Services;

public class UserAuditService<TMessage> : IMessageService<TMessage>
{
    private readonly IUserAuditRepository _userAuditRepository;
    private readonly IValidator<TMessage> _validator;
    private readonly IMapper _mapper;
    
    public UserAuditService(
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