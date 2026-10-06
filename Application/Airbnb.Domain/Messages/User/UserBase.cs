namespace Airbnb.Domain.Messages.User;

public abstract class UserBase
{
    public string? UserId { get; set; }
    public DateTime? CreatedAt { get; set; }
}