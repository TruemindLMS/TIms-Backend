
namespace TeamIndia.TalentFlow.Domain.Entities;

public class Notification
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
}
