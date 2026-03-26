namespace TeamIndia.TalentFlow.Domain.Entities;

public class Lesson
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

}
