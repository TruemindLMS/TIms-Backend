namespace TeamIndia.TalentFlow.Domain.Entities;

public class Certificate
{
    public Guid CertificateId { get; set; }
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public DateTime IssuedOnUtc { get; set; } = DateTime.UtcNow;

    public string HtmlContent { get; set; } = string.Empty;

    public string? FileUrl { get; set; }
}
