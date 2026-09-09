using Microsoft.AspNetCore.Identity;

namespace Airbnb.Domain.Models;

public class User : IdentityUser
{
    public string? ExternalUserId { get; set; }
}