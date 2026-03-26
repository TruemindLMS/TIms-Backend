using TeamIndia.TalentFlow.Domain.Common;

namespace TeamIndia.TalentFlow.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
}
