using TeamIndia.TalentFlow.Domain.Common;

namespace TeamIndia.TalentFlow.Domain.Entities;

public class LessonCompletion : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid LessonId { get; set; }
    public virtual Lesson Lesson { get; set; } = null!;

    public DateTime CompletedAtUtc { get; set; } = DateTime.UtcNow;
}
