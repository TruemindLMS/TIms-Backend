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
    }
}
