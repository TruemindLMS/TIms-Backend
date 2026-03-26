namespace TeamIndia.TalentFlow.Domain.Entities;

public class Course
{

    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public virtual ICollection<ProgressRecord> ProgressRecords { get; set; } = new List<ProgressRecord>();
}
