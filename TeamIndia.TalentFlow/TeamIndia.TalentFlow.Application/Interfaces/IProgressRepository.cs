using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface IProgressRepository
    {
        Task<int> GetTotalLessonsAsync(Guid courseId);
        Task<int> GetCompletedLessonsAsync(Guid courseId, Guid userId);
    Task<DateTime?> GetCourseCompletionDateAsync(Guid courseId, Guid userId);
        Task<int> GetTotalAssignmentsAsync(Guid courseId);
        Task<int> GetSubmittedAssignmentsAsync(Guid courseId, Guid userId);

        Task<Guid?> GetLessonCourseIdAsync(Guid lessonId);
        Task<bool> HasLessonCompletionAsync(Guid lessonId, Guid userId);
        Task AddLessonCompletionAsync(LessonCompletion completion);
        Task<bool> HasCompletedAllLessonsAsync(Guid courseId, Guid userId);
        Task UpsertProgressRecordAsync(Guid courseId, Guid userId);
        Task<List<Guid>> GetCoursesForUserAsync(Guid userId);
        // Number of courses the user is enrolled in
        Task<int> GetEnrolledCourseCountAsync(Guid userId);

        // Number of enrolled courses the user has completed
        Task<int> GetCompletedCoursesCountAsync(Guid userId);
    }
}
