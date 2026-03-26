
namespace TeamIndia.TalentFlow.Domain.Entities;

public class ProgressRecord
{
    public Guid ProgressRecordId { get; set; }
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalAssignments { get; set; }
    public int SubmittedAssignments { get; set; }
}
