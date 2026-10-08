using Airbnb.Application.Abstracts.Identity;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Broker.Messages.User;
using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Services;

public class UserService : IUserService
{
    private readonly IIdentityService _identityService;
    private readonly IEventPublisher _eventPublisher;

    public UserService(
        IIdentityService identityService,
        IEventPublisher eventPublisher)
    {
        _identityService = identityService;
        _eventPublisher = eventPublisher;
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

        await _eventPublisher.PublishAsync(new UserCreated()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            CreatedAt = DateTime.UtcNow,
        });

        return result;
    }

    public async Task<IdentityResult> UpdateEmailAsync(string userId, string newEmail)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null) return UserNotFound(userId);

        var oldEmail = user.Email;

        var result = await _identityService.ChangeEmailAsync(user, newEmail);
        if (!result.Succeeded) return result;

        await _eventPublisher.PublishAsync(new UserEmailChanged()
        {
            UserId = user.Id,
            UserName = user.UserName,
            CreatedAt = DateTime.UtcNow,
            OldEmail = oldEmail,
            NewEmail = newEmail,
        });

        return result;
    }

    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null) return UserNotFound(userId);

        var result = await _identityService.DeleteUserAsync(user);
        if (!result.Succeeded) return result;

        await _eventPublisher.PublishAsync(new UserDeleted()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            CreatedAt = DateTime.UtcNow,
        });

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
