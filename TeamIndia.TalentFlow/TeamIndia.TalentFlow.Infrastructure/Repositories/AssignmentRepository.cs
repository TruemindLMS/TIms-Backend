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
    }
}
