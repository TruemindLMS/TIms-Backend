using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface IAssignmentRepository
    {
        Task AddAssignmentAsync(Assignment assignment);
        Task<Assignment?> GetAssignmentAsync(Guid assignmentId);
        Task<IEnumerable<Assignment>> GetAssignmentsByCourseAsync(Guid courseId);

        Task AddSubmissionAsync(Submission submission);
        Task<Submission?> GetSubmissionByAssignmentAndUserAsync(Guid assignmentId, Guid userId);
        Task<int> GetTotalAssignmentsForUserAsync(Guid userId);
        Task<int> GetSubmittedAssignmentsCountAsync(Guid userId);
        Task<int> GetOverdueAssignmentsCountAsync(Guid userId);
        Task<int> GetPendingAssignmentsCountAsync(Guid userId);
        Task<(IEnumerable<Assignment> items, int total)> GetAssignmentsForUserPagedAsync(Guid userId, int page, int pageSize, string? filter, string? status);
    }
}
