using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Services;

public interface IUserManagementService
{
    Task<IdentityResult> CreateUserAsync(User user, string password);
    Task<IdentityResult> UpdateEmailAsync(User user, string newEmail);
    Task<IdentityResult> DeleteUserAsync(User user);
    Task<bool> RoleExistsAsync(string roleName);
    Task<User?> FindUserByEmailAsync(string email);
    Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<IList<string>> GetRolesAsync(User user);
}