namespace TeamIndia.TalentFlow.Domain.Entities;

public class Assignment
{
    public Guid AssignmentId { get; set; }
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public Guid? LessonId { get; set; }
    public virtual Lesson? Lesson { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDateUtc { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
