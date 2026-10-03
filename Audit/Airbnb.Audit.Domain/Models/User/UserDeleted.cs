namespace Airbnb.Audit.Domain.Models.User;

public class UserDeleted : UserBase
{
    public string? Email { get; set; }
}