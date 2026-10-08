namespace Airbnb.Contracts.Broker.Messages.User;

public class UserEmailChanged
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? NewEmail { get; set; }
    public string? OldEmail { get; set; }
    public DateTime? CreatedAt { get; set; }
}