
namespace TeamIndia.TalentFlow.Domain.Entities;

public class Enrollment
{
    public Guid EnrollmentId { get; set; }
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public DateTime EnrolledOnUtc { get; set; } = DateTime.UtcNow;
}
