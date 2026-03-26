using TeamIndia.TalentFlow.Domain.Common;

namespace TeamIndia.TalentFlow.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public DateTime EnrolledAtUtc { get; set; } = DateTime.UtcNow;
}
