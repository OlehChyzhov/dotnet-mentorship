using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Contracts.MessageHandlers;
using Airbnb.Contracts.Messages;

namespace Airbnb.Audit.Application.Services;

public class UserAuditMessageHandler : IMessageHandler<UserCreated>, IMessageHandler<UserDeleted>, IMessageHandler<UserEmailChanged>
{
    private readonly IUserAuditRepository _userAuditRepository;
    public UserAuditMessageHandler(IUserAuditRepository userAuditRepository)
    {
        _userAuditRepository = userAuditRepository;
    }
    
    public Task HandleAsync(UserCreated message)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(UserDeleted message)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(UserEmailChanged message)
    {
        throw new NotImplementedException();
    }
}