using Airbnb.Application.Abstracts.Identity;
using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public Task<IdentityResult> CreateUserAsync(User user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> ChangeEmailAsync(User user, string newEmail)
    {
        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        return await _userManager.ChangeEmailAsync(user, newEmail, token);
    }

    public Task<IdentityResult> DeleteUserAsync(User user)
    {
        return _userManager.DeleteAsync(user);
    }

    public Task<User?> FindUserByIdAsync(string userId)
    {
        return _userManager.FindByIdAsync(userId);
    }

    public Task<User?> FindUserByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public Task<bool> CheckPasswordAsync(User user, string password)
    {
        return _userManager.CheckPasswordAsync(user, password);
    }

    public Task<bool> RoleExistsAsync(string roleName)
    {
        return _roleManager.RoleExistsAsync(roleName);
    }

    public Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
    {
        return _userManager.AddToRoleAsync(user, roleName);
    }

    public Task<IList<string>> GetRolesAsync(User user)
    {
        return _userManager.GetRolesAsync(user);
    }
}
