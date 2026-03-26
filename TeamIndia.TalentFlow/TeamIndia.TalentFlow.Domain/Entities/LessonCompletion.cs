
namespace TeamIndia.TalentFlow.Domain.Entities;

public class LessonCompletion
{
    public Guid LessonCompletionId { get; set; }
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid LessonId { get; set; }
    public virtual Lesson Lesson { get; set; } = null!;

    public DateTime CompletedOnUtc { get; set; } = DateTime.UtcNow;
}
