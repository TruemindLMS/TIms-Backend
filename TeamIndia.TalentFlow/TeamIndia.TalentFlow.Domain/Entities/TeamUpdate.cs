namespace TeamIndia.TalentFlow.Domain.Entities
{
    public class TeamUpdate
    {
        public Guid TeamUpdateId { get; set; }
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual Team Team { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
