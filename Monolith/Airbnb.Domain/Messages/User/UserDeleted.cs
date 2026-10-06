namespace Airbnb.Domain.Messages.User;

public class UserDeleted : UserBase
{
    public string? Email { get; set; }
}