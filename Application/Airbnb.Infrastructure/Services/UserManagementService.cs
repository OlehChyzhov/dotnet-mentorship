using Airbnb.Application.Abstracts.Broker;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Domain.Messages;
using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEventPublisher _eventPublisher;

    public UserManagementService(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IEventPublisher eventPublisher)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _eventPublisher = eventPublisher;
    }

    public async Task<IdentityResult> CreateUserAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return result;
        
        var createdUser = await GetCreatedUser(user);
        if (createdUser != null)
        {
            var message = new UserCreatedMessage()
            {
                Id = createdUser.Id,
                Name = createdUser.UserName,
                Email = createdUser.Email,
                OccuredOn = DateTime.UtcNow,
            };
            
            await _eventPublisher.PublishAsync(message);
        }

        return result;
    }

    public async Task<IdentityResult> UpdateEmailAsync(User user, string newEmail)
    {
        var oldEmail = user.Email;
        
        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

        if (result.Succeeded)
        {
            await _eventPublisher.PublishAsync<UserEmailChangedMessage>(new UserEmailChangedMessage()
            {
                Id = user.Id,
                Name = user.UserName,
                OccuredOn = DateTime.UtcNow,
                OldEmail = oldEmail,
                NewEmail = newEmail,
            });
        }

        return result;
    }

    public async Task<IdentityResult> DeleteUserAsync(User user)
    {
        var userId = user.Id;
        var userEmail = user.Email;
        var userName = user.UserName;
        
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            var message = new UserDeletedMessage()
            {
                Id = userId,
                Name = userName,
                Email = userEmail,
                OccuredOn = DateTime.UtcNow,
            };
            
            await _eventPublisher.PublishAsync(message);
        }
        
        return result;
    }
    
    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<User?> FindUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
    {
        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public Task<IList<string>> GetRolesAsync(User user)
    {
        return _userManager.GetRolesAsync(user);
    }

    private async Task<User?> GetCreatedUser(User user)
    {
        if (!string.IsNullOrEmpty(user.Id))
        {
            return await _userManager.FindByIdAsync(user.Id);
        }
        else if (!string.IsNullOrEmpty(user.Email))
        {
            return await _userManager.FindByEmailAsync(user.Email);
        }
        else if (!string.IsNullOrEmpty(user.UserName))
        {
            return await _userManager.FindByNameAsync(user.UserName);
        }

        return null;
    }
}