namespace TeamIndia.TalentFlow.Domain.Entities
{
    public class Team
    {
        public Guid TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public virtual ICollection<TeamTask> Tasks { get; set; } = new List<TeamTask>();
        public virtual ICollection<TeamUpdate> Updates { get; set; } = new List<TeamUpdate>();
    }
}
