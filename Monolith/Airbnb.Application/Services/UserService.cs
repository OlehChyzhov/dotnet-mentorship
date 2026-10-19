using Airbnb.Application.Abstractions.Services;
using Airbnb.Application.Abstracts.Identity;
using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Broker.Messages.User;
using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Services;

public class UserService : IUserService
{
    private readonly IIdentityService _identityService;
    private readonly IPublisher _publisher;

    public UserService(
        IIdentityService identityService,
        IPublisher publisher)
    {
        _identityService = identityService;
        _publisher = publisher;
    }

    public async Task<IdentityResult> CreateUserAsync(User user, string password, string role)
    {
        if (!await _identityService.RoleExistsAsync(role))
        {
            return IdentityResult.Failed(new IdentityError()
            {
                Code = Constants.CodeRoleNotFound,
                Description = $"The role '{role}' does not exist"
            });
        }
        
        var result = await _identityService.CreateUserAsync(user, password);
        if (!result.Succeeded) return result;

        result = await _identityService.AddUserToRoleAsync(user, role);
        if (!result.Succeeded) return result;
        
        await _publisher.PublishAsync(new UserCreated() { EntityId = user.Id });
        
        return result;
    }

    public async Task<IdentityResult> UpdateEmailAsync(string userId, string newEmail)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null) return UserNotFound(userId);

        var oldEmail = user.Email;

        var result = await _identityService.ChangeEmailAsync(user, newEmail);
        if (!result.Succeeded) return result;

        var message = new UserUpdated()
        {
            EntityId = user.Id, 
            PropertyName = nameof(user.Email), 
            NewValue = user.Email, 
            OldValue = oldEmail
        };
        
        await _publisher.PublishAsync(message);
        return result;
    }

    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null) return UserNotFound(userId);

        var result = await _identityService.DeleteUserAsync(user);
        if (!result.Succeeded) return result;
        
        await _publisher.PublishAsync(new UserDeleted() { EntityId = user.Id });

        return result;
    }

    private static IdentityResult UserNotFound(string userId)
    {
        return IdentityResult.Failed(new IdentityError()
        {
            Code = Constants.CodeUserNotFound,
            Description = $"User '{userId}' does not exist"
        });
    }
}
