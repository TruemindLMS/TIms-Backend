using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Domain.Entities
{
    public class TeamTask
    {
        public Guid TeamTaskId { get; set; }
        public Guid TeamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TeamTaskStatus Status { get; set; } = TeamTaskStatus.Todo;
        public Guid? AssignedToUserId { get; set; }
        public DateTime? DueDateUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual Team Team { get; set; } = null!;
        public virtual ApplicationUser? AssignedToUser { get; set; }
    }
}
