namespace TeamIndia.TalentFlow.Domain.Entities;

public class Feedback
{
    public Guid FeedbackId { get; set; }
    public Guid SubmissionId { get; set; }
    public virtual Submission Submission { get; set; } = null!;

    public Guid ReviewerId { get; set; }
    public virtual ApplicationUser Reviewer { get; set; } = null!;

    public string Comments { get; set; } = string.Empty;
}
