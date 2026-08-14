using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IUserRepository
{
    public Task<bool> RoleExistsAsync(string roleName);

    public Task<IdentityUser?> FindUserByEmailAsync(string email);

    public Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);

    public Task<IdentityResult> AddUserToRoleAsync(IdentityUser user, string roleName);
    
    public Task<bool> CheckPasswordAsync(IdentityUser user, string password);
    
    public Task<IList<string>> GetRolesAsync(IdentityUser user);
}