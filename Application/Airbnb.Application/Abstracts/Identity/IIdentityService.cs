using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Identity;

public interface IIdentityService
{
    Task<IdentityResult> CreateUserAsync(User user, string password);
    Task<IdentityResult> ChangeEmailAsync(User user, string newEmail);
    Task<IdentityResult> DeleteUserAsync(User user);
    Task<User?> FindUserByIdAsync(string userId);
    Task<User?> FindUserByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<bool> RoleExistsAsync(string roleName);
    Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);
    Task<IList<string>> GetRolesAsync(User user);
}
