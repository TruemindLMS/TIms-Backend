using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories
{
    public class ProgressRepository : IProgressRepository
    {
        private readonly ApplicationDbContext _db;

        public ProgressRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> GetTotalLessonsAsync(Guid courseId)
        {
            return await _db.Lessons.CountAsync(l => l.CourseId == courseId);
        }

        public async Task<int> GetCompletedLessonsAsync(Guid courseId, Guid userId)
        {
            var lessonIds = await _db.Lessons.Where(l => l.CourseId == courseId).Select(l => l.LessonId).ToListAsync();
            return await _db.LessonCompletions.CountAsync(lc => lc.UserId == userId && lessonIds.Contains(lc.LessonId));
        }

        public async Task<int> GetTotalAssignmentsAsync(Guid courseId)
        {
            return await _db.Assignments.CountAsync(a => a.CourseId == courseId);
        }

        public async Task<int> GetSubmittedAssignmentsAsync(Guid courseId, Guid userId)
        {
            return await _db.Submissions.CountAsync(s => s.UserId == userId && _db.Assignments.Any(a => a.AssignmentId == s.AssignmentId && a.CourseId == courseId));
        }

        public async Task<Guid?> GetLessonCourseIdAsync(Guid lessonId)
        {
            var lesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.LessonId == lessonId);
            return lesson?.CourseId;
        }

        public async Task<bool> HasLessonCompletionAsync(Guid lessonId, Guid userId)
        {
            return await _db.LessonCompletions.AnyAsync(lc => lc.LessonId == lessonId && lc.UserId == userId);
        }

        public async Task AddLessonCompletionAsync(LessonCompletion completion)
        {
            await _db.LessonCompletions.AddAsync(completion);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> HasCompletedAllLessonsAsync(Guid courseId, Guid userId)
        {
            var total = await GetTotalLessonsAsync(courseId);
            var completed = await GetCompletedLessonsAsync(courseId, userId);
            return total > 0 && completed >= total;
        }

        public async Task UpsertProgressRecordAsync(Guid courseId, Guid userId)
        {
            var totalLessons = await GetTotalLessonsAsync(courseId);
            var completedLessons = await GetCompletedLessonsAsync(courseId, userId);
            var totalAssignments = await GetTotalAssignmentsAsync(courseId);
            var submittedAssignments = await GetSubmittedAssignmentsAsync(courseId, userId);

            var existing = await _db.ProgressRecords.FirstOrDefaultAsync(p => p.CourseId == courseId && p.UserId == userId);
            if (existing == null)
            {
                existing = new ProgressRecord
                {
                    ProgressRecordId = Guid.NewGuid(),
                    CourseId = courseId,
                    UserId = userId,
                    TotalLessons = totalLessons,
                    CompletedLessons = completedLessons,
                    TotalAssignments = totalAssignments,
                    SubmittedAssignments = submittedAssignments
                };
                await _db.ProgressRecords.AddAsync(existing);
            }
            else
            {
                existing.TotalLessons = totalLessons;
                existing.CompletedLessons = completedLessons;
                existing.TotalAssignments = totalAssignments;
                existing.SubmittedAssignments = submittedAssignments;
                _db.ProgressRecords.Update(existing);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetCoursesForUserAsync(Guid userId)
        {
            // Return courses where user is enrolled; fallback to all courses if no enrollments
            var enrolled = await _db.Enrollments
                .Where(e => e.UserId == userId)
                .Select(e => e.CourseId)
                .Distinct()
                .ToListAsync();

            if (enrolled != null && enrolled.Count > 0) return enrolled;

            return await _db.Courses.Select(c => c.CourseId).ToListAsync();
        }

        public async Task<int> GetEnrolledCourseCountAsync(Guid userId)
        {
            return await _db.Enrollments.CountAsync(e => e.UserId == userId);
        }

        public async Task<int> GetCompletedCoursesCountAsync(Guid userId)
        {
            var courseIds = await _db.Enrollments.Where(e => e.UserId == userId).Select(e => e.CourseId).ToListAsync();
            var completedCount = 0;
            foreach (var cid in courseIds)
            {
                var total = await GetTotalLessonsAsync(cid);
                if (total == 0) continue;
                var completed = await GetCompletedLessonsAsync(cid, userId);
                if (completed >= total) completedCount++;
            }
            return completedCount;
        }
    }
}
