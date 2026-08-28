using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Helpers;

public interface IUserHelper
{
    Task<bool> RoleExistsAsync(string roleName);
    Task<User?> FindUserByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(User user, string password);
    Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<IList<string>> GetRolesAsync(User user);
}