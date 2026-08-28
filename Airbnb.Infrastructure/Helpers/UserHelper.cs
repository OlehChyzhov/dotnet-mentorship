using Airbnb.Application.Abstracts.Helpers;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Infrastructure.Helpers;

public class UserHelper : IUserHelper
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserHelper(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<User?> FindUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> CreateUserAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
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
}