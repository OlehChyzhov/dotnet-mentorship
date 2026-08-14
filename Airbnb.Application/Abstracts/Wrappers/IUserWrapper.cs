using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Wrappers;

public interface IUserWrapper
{
    Task<bool> RoleExistsAsync(string roleName);
    Task<IdentityUser?> FindUserByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);
    Task<IdentityResult> AddUserToRoleAsync(IdentityUser user, string roleName);
    Task<bool> CheckPasswordAsync(IdentityUser user, string password);
    Task<IList<string>> GetRolesAsync(IdentityUser user);
}