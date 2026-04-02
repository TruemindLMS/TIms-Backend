using System.ComponentModel.DataAnnotations.Schema;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Domain.Entities;

public class UserOnboarding
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public Discipline? Discipline { get; set; }
    public Goal? Goal { get; set; }

    public bool IsComplete { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
