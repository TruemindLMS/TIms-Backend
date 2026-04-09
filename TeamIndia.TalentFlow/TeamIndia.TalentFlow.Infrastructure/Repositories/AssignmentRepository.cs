using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ApplicationDbContext _db;

        public AssignmentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAssignmentAsync(Assignment assignment)
        {
            await _db.Assignments.AddAsync(assignment);
            await _db.SaveChangesAsync();
        }

        public async Task<Assignment?> GetAssignmentAsync(Guid assignmentId)
        {
            return await _db.Assignments.Include(a => a.Submissions).FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByCourseAsync(Guid courseId)
        {
            return await _db.Assignments.Where(a => a.CourseId == courseId).ToListAsync();
        }

        public async Task AddSubmissionAsync(Submission submission)
        {
            await _db.Submissions.AddAsync(submission);
            await _db.SaveChangesAsync();
        }

        public async Task<Submission?> GetSubmissionByAssignmentAndUserAsync(Guid assignmentId, Guid userId)
        {
            return await _db.Submissions.FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.UserId == userId);
        }

        public async Task<int> GetTotalAssignmentsForUserAsync(Guid userId)
        {
            var courseIds = await _db.Enrollments.Where(e => e.UserId == userId).Select(e => e.CourseId).ToListAsync();
            return await _db.Assignments.CountAsync(a => courseIds.Contains(a.CourseId));
        }

        public async Task<int> GetSubmittedAssignmentsCountAsync(Guid userId)
        {
            return await _db.Submissions.CountAsync(s => s.UserId == userId);
        }

        public async Task<int> GetOverdueAssignmentsCountAsync(Guid userId)
        {
            var courseIds = await _db.Enrollments.Where(e => e.UserId == userId).Select(e => e.CourseId).ToListAsync();
            var now = DateTime.UtcNow;
            return await _db.Assignments.Where(a => courseIds.Contains(a.CourseId) && a.DueDateUtc < now && !_db.Submissions.Any(s => s.AssignmentId == a.AssignmentId && s.UserId == userId)).CountAsync();
        }

        public async Task<int> GetPendingAssignmentsCountAsync(Guid userId)
        {
            var total = await GetTotalAssignmentsForUserAsync(userId);
            var submitted = await GetSubmittedAssignmentsCountAsync(userId);
            var overdue = await GetOverdueAssignmentsCountAsync(userId);
            return Math.Max(0, total - submitted - overdue);
        }


        public async Task<(IEnumerable<Assignment> items, int total)> GetAssignmentsForUserPagedAsync(Guid userId, int page, int pageSize, string? filter, string? status)
        {
            var courseIds = await _db.Enrollments.Where(e => e.UserId == userId).Select(e => e.CourseId).ToListAsync();
            var query = _db.Assignments.Include(a => a.Submissions).Where(a => courseIds.Contains(a.CourseId)).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var f = filter.Trim().ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(f) || a.Description.ToLower().Contains(f));
            }

            var now = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals("overdue", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(a => a.DueDateUtc < now && !_db.Submissions.Any(s => s.AssignmentId == a.AssignmentId && s.UserId == userId));
                else if (status.Equals("submitted", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(a => _db.Submissions.Any(s => s.AssignmentId == a.AssignmentId && s.UserId == userId));
                else if (status.Equals("pending", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(a => a.DueDateUtc >= now && !_db.Submissions.Any(s => s.AssignmentId == a.AssignmentId && s.UserId == userId));
            }

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(a => a.DueDateUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }
    }
}
