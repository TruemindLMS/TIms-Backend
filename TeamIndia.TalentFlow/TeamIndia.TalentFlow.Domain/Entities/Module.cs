using TeamIndia.TalentFlow.Domain.Common;

namespace TeamIndia.TalentFlow.Domain.Entities;

public class Module : BaseEntity
{
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
