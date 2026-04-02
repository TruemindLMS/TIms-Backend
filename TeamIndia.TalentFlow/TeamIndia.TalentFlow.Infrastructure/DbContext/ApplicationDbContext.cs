using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Infrastructure.DbContext;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<LessonCompletion> LessonCompletions => Set<LessonCompletion>();
    public DbSet<ProgressRecord> ProgressRecords => Set<ProgressRecord>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserOnboarding> UserOnboardings => Set<UserOnboarding>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<TeamTask> TeamTasks => Set<TeamTask>();
    public DbSet<TeamUpdate> TeamUpdates => Set<TeamUpdate>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserOnboarding>()
            .HasOne(o => o.User)
            .WithOne()
            .HasForeignKey<UserOnboarding>(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Team>()
                .HasKey(x => x.TeamId);

        builder.Entity<TeamMember>()
            .HasKey(x => x.TeamMemberId);

        builder.Entity<TeamTask>()
            .HasKey(x => x.TeamTaskId);

        builder.Entity<TeamUpdate>()
            .HasKey(x => x.TeamUpdateId);

        builder.Entity<TeamMember>()
            .HasIndex(x => new { x.TeamId, x.UserId })
            .IsUnique();

        builder.Entity<TeamMember>()
            .HasOne(x => x.Team)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TeamMember>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TeamTask>()
            .HasOne(x => x.Team)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TeamTask>()
            .HasOne(x => x.AssignedToUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TeamUpdate>()
            .HasOne(x => x.Team)
            .WithMany(x => x.Updates)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TeamUpdate>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
