using Airbnb.Application.Abstracts.Helpers;
using Airbnb.Application.Abstracts.Services;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Infrastructure.Helpers;

public class UserHelper : IUserHelper
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserHelper(
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<IdentityUser?> FindUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> AddUserToRoleAsync(IdentityUser user, string roleName)
    {
        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public Task<IList<string>> GetRolesAsync(IdentityUser user)
    {
        return _userManager.GetRolesAsync(user);
    }
}