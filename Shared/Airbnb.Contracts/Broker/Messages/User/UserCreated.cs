namespace Airbnb.Contracts.Broker.Messages.User;

public class UserCreated
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedAt { get; set; }
}