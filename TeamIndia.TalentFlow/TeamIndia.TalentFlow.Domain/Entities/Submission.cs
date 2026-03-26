using TeamIndia.TalentFlow.Domain.Common;

namespace TeamIndia.TalentFlow.Domain.Entities;

public class Submission : BaseEntity
{
    public Guid AssignmentId { get; set; }
    public virtual Assignment Assignment { get; set; } = null!;

    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public string? TextResponse { get; set; }
    public string? FileUrl { get; set; }
    public string? LinkUrl { get; set; }

    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
