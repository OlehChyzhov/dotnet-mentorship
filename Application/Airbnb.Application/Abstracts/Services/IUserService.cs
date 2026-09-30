using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Services;

public interface IUserService
{
    Task<IdentityResult> CreateUserAsync(User user, string password, string role);
    Task<IdentityResult> UpdateEmailAsync(string userId, string newEmail);
    Task<IdentityResult> DeleteUserAsync(string userId);
}
