using System.ComponentModel.DataAnnotations.Schema;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? Location { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
