namespace Airbnb.Audit.Domain.Models.User;

public abstract class UserBase
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public DateTime? CreatedAt { get; set; }
}