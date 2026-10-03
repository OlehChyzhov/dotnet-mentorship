namespace Airbnb.Audit.Domain.Models.User;

public class UserEmailChanged : UserBase
{
    public string? NewEmail { get; set; }
    public string? OldEmail { get; set; }
}