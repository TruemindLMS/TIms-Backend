using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos.Request;
using TeamIndia.TalentFlow.Application.Dtos.Response;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface IAssignmentService
    {
        Task<BaseResponse<AssignmentResponseDto>> CreateAssignmentAsync(CreateAssignmentRequestDto dto, Guid mentorId);
        Task<BaseResponse<SubmissionResponseDto>> SubmitAssignmentAsync(CreateSubmissionRequestDto dto, Guid userId);
        Task<BaseResponse<AssignmentResponseDto>> GetAssignmentAsync(Guid assignmentId);
        Task<BaseResponse<object>> GetAssignmentSummaryForUserAsync(Guid userId);
        Task<BaseResponse<object>> GetAssignmentsForUserPagedAsync(Guid userId, int page, int pageSize, string? filter, string? status);
    }
}
